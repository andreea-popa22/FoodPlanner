namespace FoodPlaner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterrecipe : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Recipes", "Intolerances", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Recipes", "Intolerances", c => c.Int(nullable: false));
        }
    }
}
