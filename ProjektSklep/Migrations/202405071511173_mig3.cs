namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        warehouseProductId = c.Int(nullable: false, identity: true),
                        productId = c.Int(nullable: false),
                        actualState = c.Int(nullable: false),
                        stockLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.warehouseProductId)
                .ForeignKey("dbo.Products", t => t.productId, cascadeDelete: true)
                .Index(t => t.productId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Warehouses", "productId", "dbo.Products");
            DropIndex("dbo.Warehouses", new[] { "productId" });
            DropTable("dbo.Warehouses");
        }
    }
}
