using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityControl.API.Extensions;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace IdentityControl.API.Data
{
    public class IdentityRepository<T> : IIdentityRepository<T> where T : class
    {
        private readonly ConfigurationDbContext _context;
        private readonly IEventSender _eventSender;

        public IdentityRepository(ConfigurationDbContext context, IEventSender eventSender)
        {
            _context = context;
            _eventSender = eventSender;
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            if (entity == null)
            {
                throw new Exception($"The entity of type {typeof(T)} with id {id} was not found.");
            }

            return entity;
        }

        public void Insert(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public async Task InsertAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public void Delete(object id)
        {
            var entityToDelete = _context.Set<T>().Find(id);
            Delete(entityToDelete);
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }

            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveAsync(IToasterEvent toasterEvent, int expectedResult = 1)
        {
            var result = await _context.SaveChangesAsync();

            if (result == expectedResult)
            {
                var success = EventBuilder.BuildToasterEvent(toasterEvent);
                await _eventSender.SendAsync(success.ToJson());
            }
            else
            {
                var failure = toasterEvent.TransformInFailure();
                await _eventSender.SendAsync(failure);
            }

            return result;
        }
    }
}