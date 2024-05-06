namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imageTest1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "image");
        }
    }
}
