namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductOrders", "count", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductOrders", "count");
        }
    }
}
