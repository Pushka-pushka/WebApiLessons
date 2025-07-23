using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using  HotelsWebApi.Model;
using HotelsWebApi.Domain;

namespace HotelsWebApi.Domain.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelDb _context;
        private bool _disposed = false;


        public HotelRepository(HotelDb context)
        {
            _context = context;
        }

        public Task<List<Hotel>> GetHotelsAsync() => _context.Hotels.ToListAsync();

        public async Task<Hotel> GetHotelAsync(int hotelId) =>
            await _context.Hotels.FindAsync(new object[]{hotelId});
        
        public async Task InsertHotelAsync(Hotel hotel) => await _context.Hotels.AddAsync(hotel);

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            var hotelFromDb = await _context.Hotels.FindAsync(new object[]{hotel.Id});
            if(hotelFromDb == null) return;

            hotelFromDb.Name = hotel.Name;
            hotelFromDb.Latitude = hotel.Latitude;
            hotelFromDb.Longitude = hotel.Longitude;
        }

        public async Task DeleteHotelAsync(int hotelId)
        {
            var hotelFromDb = await _context.Hotels.FindAsync(new object[]{hotelId});
            if(hotelFromDb == null) return;
            _context.Hotels.Remove(hotelFromDb);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if(disposing) 
                { 
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
    }
}