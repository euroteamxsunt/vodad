﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VodadModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class VodadEntities : ObjectContext
    {
        public VodadEntities()
            : base("name=VodadEntities", "VodadEntities")
        {
        }
    
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }*/
    
        public DbSet<C__RefactorLog> C__RefactorLog { get; set; }
        public DbSet<AlertMessage> AlertMessage { get; set; }
        public DbSet<Ban> Ban { get; set; }
        public DbSet<BlackList> BlackList { get; set; }
        public DbSet<Certificates> Certificates { get; set; }
        public DbSet<Cheaters> Cheaters { get; set; }
        public DbSet<Geolocation> Geolocation { get; set; }
        public DbSet<GeolocationPlatformPercentage> GeolocationPlatformPercentage { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Merchants> Merchants { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<MoneyTransfers> MoneyTransfers { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderContent> OrderContent { get; set; }
        public DbSet<OrderPerformed> OrderPerformed { get; set; }
        public DbSet<OrderThemes> OrderThemes { get; set; }
        public DbSet<PerformerPlatform> PerformerPlatform { get; set; }
        public DbSet<PerformerStatistics> PerformerStatistics { get; set; }
        public DbSet<Regions> Regions { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Themes> Themes { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<TicketThemes> TicketThemes { get; set; }
        public DbSet<Timezone> Timezone { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserMerchants> UserMerchants { get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<Wallet> Wallet { get; set; }
        public DbSet<WhiteList> WhiteList { get; set; }
    
        public virtual int pr_CreateFkTreeDeleteTrigger(string schema_name, Nullable<bool> is_verbose)
        {
            var schema_nameParameter = schema_name != null ?
                new ObjectParameter("schema_name", schema_name) :
                new ObjectParameter("schema_name", typeof(string));
    
            var is_verboseParameter = is_verbose.HasValue ?
                new ObjectParameter("is_verbose", is_verbose) :
                new ObjectParameter("is_verbose", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("pr_CreateFkTreeDeleteTrigger", schema_nameParameter, is_verboseParameter);
        }
    
        public virtual int pr_FkTreeDelete(string parent_table_id, string where_clause, string from_clause, Nullable<int> cascate_level, Nullable<bool> is_verbose)
        {
            var parent_table_idParameter = parent_table_id != null ?
                new ObjectParameter("parent_table_id", parent_table_id) :
                new ObjectParameter("parent_table_id", typeof(string));
    
            var where_clauseParameter = where_clause != null ?
                new ObjectParameter("where_clause", where_clause) :
                new ObjectParameter("where_clause", typeof(string));
    
            var from_clauseParameter = from_clause != null ?
                new ObjectParameter("from_clause", from_clause) :
                new ObjectParameter("from_clause", typeof(string));
    
            var cascate_levelParameter = cascate_level.HasValue ?
                new ObjectParameter("cascate_level", cascate_level) :
                new ObjectParameter("cascate_level", typeof(int));
    
            var is_verboseParameter = is_verbose.HasValue ?
                new ObjectParameter("is_verbose", is_verbose) :
                new ObjectParameter("is_verbose", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("pr_FkTreeDelete", parent_table_idParameter, where_clauseParameter, from_clauseParameter, cascate_levelParameter, is_verboseParameter);
        }
    
        public virtual int pr_PopulateStaticData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("pr_PopulateStaticData");
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    }
}