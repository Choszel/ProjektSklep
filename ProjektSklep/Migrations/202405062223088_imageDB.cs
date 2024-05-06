namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imageDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "imageId", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "imageId");
            AddForeignKey("dbo.Products", "imageId", "dbo.Images", "imageId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "imageId", "dbo.Images");
            DropIndex("dbo.Products", new[] { "imageId" });
            DropColumn("dbo.Products", "imageId");
        }
    }
}
