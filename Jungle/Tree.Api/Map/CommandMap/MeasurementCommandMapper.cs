using Tree.Api.Model;
using Tree.Api.Model.Measurement;
using Tree.App.Administration.Handler;
using Tree.App.Core.Exception;
using Tree.App.Measurement;
using Tree.Domain.Model.Measurement;
using ExpressMapper;
using ExpressMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tree.Api.Map.CommandMap {
    public class MeasurementCommandMapper : ICommandMapper<FullMeasurementCommand, Domain.Model.Measurement.Measurement> {
        private readonly IMeasurementHandler measurementHandler;
        private readonly IJobHandler jobHandler;

        public MeasurementCommandMapper(IMeasurementHandler measurementHandler,
                                        IJobHandler jobHandler) {
            this.measurementHandler = measurementHandler;
            this.jobHandler = jobHandler;
        }

        public Domain.Model.Measurement.Measurement Map(FullMeasurementCommand command) {
            var measurementCommand = command.Measurement;

            var existingMeasurement = measurementHandler.Get(
                      x => x.ToolId == measurementCommand.ToolId &&
                      x.Installed == measurementCommand.Installed,
                      "Job", "CreatedBy", "UpdatedBy").FirstOrDefault();

            if (command.CommandType == CommandType.Create) {
                if (existingMeasurement == null) {
                    return MapAsCreate(command);
                }
                return MapAsAppend(command, existingMeasurement);
            }
            else {
                if (existingMeasurement == null) {
                    throw new NoMatchingEntityException("Database", "No matching active measurement found");
                }
                return MapAsUpdate(command, existingMeasurement);
            }
        }

        private Domain.Model.Measurement.Measurement MapAsCreate(FullMeasurementCommand command) {
            var measurementCommand = command.Measurement;

            var job = jobHandler.Get(command.JobId);

            var now = DateTimeOffset.UtcNow;
            
            return new Domain.Model.Measurement.Measurement {
                Id = Guid.NewGuid(),
                ToolId = measurementCommand.ToolId,
                Created = now,
                Updated = now,
                Installed = measurementCommand.Installed,
                IsActive = true,
                Job = job,
                Inspections = new List<Domain.Model.Measurement.Inspection>(),
                CrimpLocation = measurementCommand.CrimpLocation,
                IsMountSuccessfull = measurementCommand.IsMountSuccessfull,
                Connection = measurementCommand.Connection.Map<Api.Model.Measurement.Connection, Domain.Model.Measurement.Connection>(),
                FinalDiameter = measurementCommand.FinalDiameter.Map<Dimension<DiameterUnit>, Domain.Model.Measurement.DiameterDimension>(),
                Pressure = measurementCommand.Pressure.Map<Dimension<PressureUnit>, Domain.Model.Measurement.PressureDimension>(),
                Torque = measurementCommand.Torque.Map<Dimension<TorqueUnit>, Domain.Model.Measurement.TorqueDimension>(),
                CreatedBy = null, // must be assigned elsewhere
                UpdatedBy = null
            };
        }

        private Domain.Model.Measurement.Measurement MapAsAppend(FullMeasurementCommand command, Domain.Model.Measurement.Measurement existingMeasurement) {
            return MapAsUpdate(command, existingMeasurement, true);
        }

        private Domain.Model.Measurement.Measurement MapAsUpdate(FullMeasurementCommand command, Domain.Model.Measurement.Measurement existingMeasurement) {
            return MapAsUpdate(command, existingMeasurement, false);
        }

        private Domain.Model.Measurement.Measurement MapAsUpdate(FullMeasurementCommand command, Domain.Model.Measurement.Measurement existingMeasurement, bool appendOnly) {
            var measurementCommand = command.Measurement;

            var result = new Domain.Model.Measurement.Measurement();
            Mapper.Map(existingMeasurement, result); // copy object

            // update fields only if value is actually different
            var wasActuallyUpdated = false;

            if (!appendOnly && command.JobId != existingMeasurement.Job.Id) {
                result.Job = jobHandler.Get(command.JobId);
                wasActuallyUpdated = true;
            }

            if (existingMeasurement.CrimpLocation != measurementCommand.CrimpLocation &&
                (!appendOnly || string.IsNullOrEmpty(existingMeasurement.CrimpLocation))) {
                result.CrimpLocation = measurementCommand.CrimpLocation;
                wasActuallyUpdated = true;
            }

            if (!appendOnly || (measurementCommand.Torque != null &&
                                (existingMeasurement.Torque == null ||
                                 existingMeasurement.Torque.Unit == null ||
                                 existingMeasurement.Torque.Value == null))) {
                // update to empty Torque
                if (measurementCommand.Torque == null) {
                    result.Torque = new TorqueDimension {
                        Unit = null,
                        Value = null
                    };
                    if (existingMeasurement.Torque != null &&
                        (existingMeasurement.Torque.Unit != null || existingMeasurement.Torque.Value != null)) {
                        wasActuallyUpdated = true;
                    }
                }
                // update/append to non-empty Torque
                else {
                    result.Torque = measurementCommand.Torque.Map<Dimension<TorqueUnit>, Domain.Model.Measurement.TorqueDimension>();
                    if (existingMeasurement == null ||
                        (result.Torque.Unit != existingMeasurement.Torque.Unit || result.Torque.Value != existingMeasurement.Torque.Value)) {
                        wasActuallyUpdated = true;
                    }
                }
            }

            // if no field was actually updated we won't send measurement to the database
            if (!wasActuallyUpdated) {
                return null;
            }

            result.Updated = DateTimeOffset.UtcNow;
            result.UpdatedBy = null; // to be assigned elsewhere
            return result;
        }
    }
}