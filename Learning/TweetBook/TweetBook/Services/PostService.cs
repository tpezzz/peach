using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Domain;
using Microsoft.EntityFrameworkCore;

namespace TweetBook.Services
{
    public class PostService : IPostService
    {

        private readonly DataContext _dataContext;

        //public PostService()
        //{
        //    _posts = new List<Post>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        _posts.Add(new Post
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = $"Post Name {i}"
        //        });
        //    }
        //}

        private readonly List<Post> _posts;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await _dataContext.Posts.AddAsync(post);

            var created = await _dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _dataContext.Posts.ToListAsync();
            //return  _posts;
        }

        public async Task<bool> UpdatePostAsync(Post updatedPost)
        {
            //var exists = GetPostById(updatedPost.Id)!= null;

            //if (!exists)
            //{
            //    return false;
            //}

            //var index = _posts.FindIndex(x => x.Id == updatedPost.Id);
            //_posts[index] = updatedPost;

             _dataContext.Posts.Update(updatedPost);
            var updated =  await _dataContext.SaveChangesAsync();

            return updated >0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);

            if (post == null) return false;

            //_posts.Remove(post);

            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();

            return deleted >0;
        }
    }
}
