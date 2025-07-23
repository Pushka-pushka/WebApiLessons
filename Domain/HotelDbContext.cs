using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using  HotelsWebApi.Model;

namespace HotelsWebApi.Domain
{
    public class HotelDb : DbContext
    {
        public HotelDb(DbContextOptions<HotelDb>options): base(options){}
        public DbSet<Hotel> Hotels => Set<Hotel>();   
    }
}