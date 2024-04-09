namespace ProjektSklep.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fsfd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        categoryId = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.categoryId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        orderId = c.Int(nullable: false, identity: true),
                        userId = c.Int(nullable: false),
                        country = c.String(nullable: false),
                        city = c.String(nullable: false),
                        street = c.String(nullable: false),
                        zipCode = c.Int(nullable: false),
                        totalPrice = c.Single(nullable: false),
                        discount = c.Int(),
                    })
                .PrimaryKey(t => t.orderId)
                .ForeignKey("dbo.Users", t => t.userId, cascadeDelete: true)
                .Index(t => t.userId);
            
            CreateTable(
                "dbo.ProductOrders",
                c => new
                    {
                        productOrderId = c.Int(nullable: false, identity: true),
                        orderId = c.Int(nullable: false),
                        productId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.productOrderId)
                .ForeignKey("dbo.Orders", t => t.orderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.productId, cascadeDelete: true)
                .Index(t => t.orderId)
                .Index(t => t.productId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        productId = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 30),
                        price = c.Single(nullable: false),
                        description = c.String(),
                        categoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.productId)
                .ForeignKey("dbo.Categories", t => t.categoryId, cascadeDelete: true)
                .Index(t => t.categoryId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        userId = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        login = c.String(nullable: false),
                        password = c.String(nullable: false),
                        email = c.String(nullable: false),
                        type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.userId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "userId", "dbo.Users");
            DropForeignKey("dbo.ProductOrders", "productId", "dbo.Products");
            DropForeignKey("dbo.Products", "categoryId", "dbo.Categories");
            DropForeignKey("dbo.ProductOrders", "orderId", "dbo.Orders");
            DropIndex("dbo.Products", new[] { "categoryId" });
            DropIndex("dbo.ProductOrders", new[] { "productId" });
            DropIndex("dbo.ProductOrders", new[] { "orderId" });
            DropIndex("dbo.Orders", new[] { "userId" });
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.ProductOrders");
            DropTable("dbo.Orders");
            DropTable("dbo.Categories");
        }
    }
}
