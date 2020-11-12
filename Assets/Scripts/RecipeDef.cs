using System.Collections.Generic;
using UnityEngine;

namespace Items {
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Items/Recipe", order = 0)]
    public class RecipeDef : ScriptableObject {
        public string recipeName;
        public string recipeDescription;
        public List<ItemDef> requiredItems;
        public List<ItemDef> returnedItems;
    }
}