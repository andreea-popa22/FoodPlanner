namespace FoodPlaner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class readdintolerances : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Intolerances", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "Intolerances");
        }
    }
}
