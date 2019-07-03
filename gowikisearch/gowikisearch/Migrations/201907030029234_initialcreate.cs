namespace gowikisearch.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WikipediaPageTitle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Popularity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Title, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.WikipediaPageTitle", new[] { "Title" });
            DropTable("dbo.WikipediaPageTitle");
        }
    }
}
