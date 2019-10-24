using Tree.Api.Map.CommandMap;
using Tree.Api.Model;
using Tree.Api.Model.Claims;
using Tree.Api.Model.Measurement;
using Tree.App.Administration.Handler;
using Tree.App.Authorization.Handler;
using Tree.App.Core.Exception;
using Tree.App.Measurement;
using Tree.Domain.Model.User;
using Tree.Domain.Validation;
using ExpressMapper.Extensions;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using DomainInspection = Tree.Domain.Model.Measurement.Inspection;
using DomainMeasurement = Tree.Domain.Model.Measurement.Measurement;

namespace Tree.Api.Controller {
    public class MeasurementController : ApiController {
        private readonly IMeasurementHandler measurementHandler;
        private readonly IInspectionHandler inspectionHandler;
        private readonly IAuthorizationHandler authorizationHandler;
        private readonly IUserHandler userHandler;
        private readonly IJobHandler jobHandler;
        private readonly ICommandMapper<FullMeasurementCommand, DomainMeasurement> measurementCommandMapper;
        private readonly ICommandMapper<FullInspectionCommand, DomainInspection> inspectionCommandMapper;
        private readonly IValidatorFactory validatorFactory;

        public MeasurementController(IMeasurementHandler measurementHandler,
                                     IInspectionHandler inspectionHandler,
                                     IAuthorizationHandler authorizationHandler,
                                     IJobHandler jobHandler,
                                     IUserHandler userHandler,
                                     ICommandMapper<FullMeasurementCommand, DomainMeasurement> measurementCommandMapper,
                                     ICommandMapper<FullInspectionCommand, DomainInspection> inspectionCommandMapper,
                                     IValidatorFactory validatorFactory) {
            this.measurementHandler = measurementHandler;
            this.inspectionHandler = inspectionHandler;
            this.authorizationHandler = authorizationHandler;
            this.jobHandler = jobHandler;
            this.userHandler = userHandler;
            this.measurementCommandMapper = measurementCommandMapper;
            this.inspectionCommandMapper = inspectionCommandMapper;
            this.validatorFactory = validatorFactory;
        }

        [Authorize(Roles = "SystemAdmin,Supervisor,Installer,Inspector,Subscriber")]
        public IHttpActionResult Get([FromUri] MeasurementQuery query) {
            var authorizationClaims = User.Identity.Map<IIdentity, AuthorizationClaims>();
            var measurements = measurementHandler.Get(authorizationClaims.CompanyId, authorizationClaims.Id, authorizationClaims.Role, query.JobId);

            if (authorizationClaims.Role == RoleType.Installer)
                measurements = measurements.Where(x => !x.Job.IsCompleted);

            var groupedMeasurements = measurements.GroupBy(x => x.Job);

            var queryResult = groupedMeasurements.Map<IEnumerable<IGrouping<Domain.Model.Company.Job, Domain.Model.Measurement.Measurement>>, List<MeasurementQueryResult>>();
            return Ok(queryResult);
        }

        [Authorize(Roles = "SystemAdmin,Supervisor,Installer,Inspector")]
        public async Task<IHttpActionResult> Post(MeasurementPackage package) {
            var authorizationClaims = User.Identity.Map<IIdentity, AuthorizationClaims>();
            var authorization = authorizationHandler.GetAuthorization(authorizationClaims.Id);

            ValidateRoleForCommandType(package.CommandType, authorization.Role);

            ValidateJobIsAuthorized(authorization, package, package.CommandType);

            if (authorization.Role == RoleType.Inspector) {
                var fullInspectionCommands = await MapToFullCommands<FullInspectionCommand>(package);

                // here we can enter Queue
                var user = userHandler.Get(authorizationClaims.Id);
                return await PostInspections(fullInspectionCommands, user);
            }
            else {
                var fullMeasurementCommands = await MapToFullCommands<FullMeasurementCommand>(package);

                // here we can enter Queue
                var user = userHandler.Get(authorizationClaims.Id);
                return await PostMeasurements(fullMeasurementCommands, user, package.CommandType);
            }
        }

        private void ValidateJobIsAuthorized(Domain.Model.Authorization.Authorization authorization, MeasurementPackage package, CommandType commandType) {
            var authorizedJobs = authorizationHandler.GetAuthorizedJobs(authorization);
            var authorizedTargetJob = authorizedJobs.FirstOrDefault(x => x.Id == package.JobId);

            if (authorizedTargetJob == null) {
                throw new PermissionException("JobId", "Provided JobId is incorrect or you are not authorized to the job with provided JobId.");
            }

            //validate for switch crimps between jobs
            if (commandType == CommandType.Update)
                foreach (var measurement in package.Measurements) {
                    var existingDomainMeasurement = measurementHandler.Get(x => x.ToolId == measurement.ToolId && x.Installed == measurement.Installed, "Job").FirstOrDefault();

                    if (existingDomainMeasurement == null)
                        throw new NoMatchingEntityException("Measurement", "Measurement not found.");

                    var authorizedCurrentJob = authorizedJobs.FirstOrDefault(x => x.Id == existingDomainMeasurement.Job.Id);

                    if (authorizedCurrentJob == null)
                        throw new PermissionException("Measurement", "You are not authorized to edit this measurement.");
                }

            // installer cannot append data to a completed job
            // inspector can add inspection to a completed job
            if (authorizedTargetJob.IsCompleted && authorization.Role != RoleType.Inspector &&
                                                   authorization.Role != RoleType.SystemAdmin &&
                                                   authorization.Role != RoleType.Supervisor) {
                throw new AppValidationException("Measurement", "Cannot add or modify measurement in a completed Job.");
            }
        }

        private void ValidateRoleForCommandType(CommandType commandType, RoleType userRole) {
            switch (commandType) {
                case CommandType.Update:
                    switch (userRole) {
                        case RoleType.Inspector:
                            throw new AppValidationException("Inspection", "Inspections cannot be updated.");
                        case RoleType.Supervisor:
                        case RoleType.SystemAdmin:
                            return; // OK
                        default:
                            throw new PermissionException("Authorization", "Only System Administrator and Supervisor can update measurement data.");
                    }
                case CommandType.Create:
                    return; // OK
                default:
                    throw new AppValidationException("CommandType", "Unknown command type.");
            }
        }

        private async Task<IHttpActionResult> PostMeasurements(IEnumerable<FullMeasurementCommand> fullMeasurementCommands, User user, CommandType commandType) {
            var domainMeasurements = fullMeasurementCommands.Select(x => measurementCommandMapper.Map(x))
                                                            .Where(x => x != null) // filter out measurements with nothing new to update
                                                            .Select(x => {
                                                                x.UpdatedBy = user;
                                                                return x;
                                                            });

            switch (commandType) {
                case CommandType.Create:
                    await measurementHandler.SaveAsync(domainMeasurements);
                    break;

                case CommandType.Update:
                    await measurementHandler.UpdateAsync(domainMeasurements);
                    break;
            }
            return Ok();
        }

        private async Task<IHttpActionResult> PostInspections(IEnumerable<FullInspectionCommand> fullInspectionCommands, User user) {
            var domainInspections = fullInspectionCommands.Select(x => inspectionCommandMapper.Map(x));

            domainInspections = domainInspections.Select(x => { x.Inspector = user; return x; });
            await inspectionHandler.CreateAsync(domainInspections);
            return Ok();
        }

        private async Task<IEnumerable<TCommand>> MapToFullCommands<TCommand>(MeasurementPackage package) {
            var commands = package.Map<MeasurementPackage, IEnumerable<TCommand>>();
            var validationResult = await validatorFactory.ValidateManyAsync(commands);
            if (!validationResult.IsValid) {
                throw new AppValidationException(validationResult.Errors);
            }
            return commands;
        }
    }
}