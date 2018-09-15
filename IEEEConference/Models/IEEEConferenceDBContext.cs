using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IEEEConference.Models
{
    public class IEEEConferenceDBContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public IEEEConferenceDBContext() : base("name=IEEEConferenceDBContext")
        {
        }

        public System.Data.Entity.DbSet<IEEEConference.Models.CommonDropList> CommonDropLists { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.UserInformation> UserInformations { get; set; }

        public System.Data.Entity.DbSet<IEEEConference.Models.Conference> Conferences { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.ControlDate> ControlDates { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.Field> Fields { get; set; }

        public System.Data.Entity.DbSet<IEEEConference.Models.PriceItem> PriceItems { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.ConfigureVPC> ConfigureVPCs { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.Committee> Committees { get; set; }

        public System.Data.Entity.DbSet<IEEEConference.Models.Paper> Papers { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<IEEEConference.Models.Author> Authors { get; set; }
        public System.Data.Entity.DbSet<IEEEConference.Models.Observation> Observations { get; set; }
        
    }
}
