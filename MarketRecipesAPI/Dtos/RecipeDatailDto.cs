namespace MarketRecipesAPI.Dtos
{
    public class RecipeDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TotalCost { get; set; }
        public List<IngredientDto> Ingredients { get; set; }

        public class IngredientDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Cost { get; set; }
            public string Unit { get; set; }
            public float Quantity { get; set; }
        }
    }
}
