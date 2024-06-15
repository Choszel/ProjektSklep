namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TomaszoZmiany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "lastSpin", c => c.DateTime());
            AddColumn("dbo.Users", "currDiscount", c => c.String());
            AlterColumn("dbo.Orders", "discount", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "discount", c => c.Int());
            DropColumn("dbo.Users", "currDiscount");
            DropColumn("dbo.Users", "lastSpin");
        }
    }
}
