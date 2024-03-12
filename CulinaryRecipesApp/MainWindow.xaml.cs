using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace CulinaryRecipesApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Recipe
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Cuisine { get; set; }
            public ObservableCollection<string> Ingredients { get; set; }
            public ObservableCollection<string> Instructions { get; set; }
            public string ImagePath { get; set; }
        }

        public class RecipeManager
        {
            public ObservableCollection<Recipe> Recipes { get; set; }

            public RecipeManager()
            {
                Recipes = new ObservableCollection<Recipe>();
            }

            public void AddRecipe(Recipe recipe)
            {
                Recipes.Add(recipe);
            }

            public void RemoveRecipe(Recipe recipe)
            {
                Recipes.Remove(recipe);
            }

            public ObservableCollection<Recipe> SearchRecipes(string keyword)
            {
                ObservableCollection<Recipe> searchResults = new ObservableCollection<Recipe>();

                foreach (var recipe in Recipes)
                {
                    // Проверка наличия ключевого слова в различных полях рецепта
                    if (recipe.Name.ToLower().Contains(keyword.ToLower()) ||
                        recipe.Type.ToLower().Contains(keyword.ToLower()) ||
                        recipe.Cuisine.ToLower().Contains(keyword.ToLower()) ||
                        recipe.Ingredients.Any(ingredient => ingredient.ToLower().Contains(keyword.ToLower())) ||
                        recipe.Instructions.Any(instruction => instruction.ToLower().Contains(keyword.ToLower())))
                    {
                        searchResults.Add(recipe);
                    }
                }

                return searchResults;
            }
        }

        public class MainViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private Recipe selectedRecipe;
            public Recipe SelectedRecipe
            {
                get { return selectedRecipe; }
                set
                {
                    if (selectedRecipe != value)
                    {
                        selectedRecipe = value;
                        OnPropertyChanged(nameof(SelectedRecipe));
                        OnPropertyChanged(nameof(IsRecipeSelected));
                        OnPropertyChanged(nameof(IsRecipeNotSelected));
                    }
                }
            }

            public bool IsRecipeSelected => SelectedRecipe != null;
            public bool IsRecipeNotSelected => !IsRecipeSelected;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public RecipeManager Manager { get; set; }

            public void ClearSearchResults(ListBox recipeListBox)
            {
                // Сброс результатов поиска и обновление отображаемого списка рецептов
                recipeListBox.ItemsSource = Manager.Recipes;
            }

            public MainViewModel()
            {
                Manager = new RecipeManager();

                Manager.AddRecipe(new Recipe
                {
                    Name = "Салат Цезарь",
                    Type = "Салат",
                    Cuisine = "Итальянская",
                    Ingredients = new ObservableCollection<string> { "Куриное филе", "Салат Романо", "Сыр Пармезан", "Гренки", "Соус Цезарь" },
                    Instructions = new ObservableCollection<string> { "Пожарить куриное филе", "Порвать салат Романо", "Посыпать сыром Пармезан", "Добавить гренки", "Заправить соусом Цезарь" },
                    ImagePath = "/Images/CaesarSalad.jpg"
                });

                Manager.AddRecipe(new Recipe
                {
                    Name = "Борщ",
                    Type = "Суп",
                    Cuisine = "Украинская",
                    Ingredients = new ObservableCollection<string> { "Свекла", "Картошка", "Морковь", "Капуста", "Мясо", "Лук", "Томатная паста" },
                    Instructions = new ObservableCollection<string> { "Нарезать овощи и мясо", "Варить варенье", "Добавить томатную пасту", "Подавать горячим" },
                    ImagePath = ""
                });

                Manager.AddRecipe(new Recipe
                {
                    Name = "Паста карбонара",
                    Type = "Макароны",
                    Cuisine = "Итальянская",
                    Ingredients = new ObservableCollection<string> { "Спагетти", "Бекон", "Яйцо", "Пармезан", "Чеснок", "Соль", "Перец" },
                    Instructions = new ObservableCollection<string> { "Сварить спагетти", "Обжарить бекон", "Смешать яйцо, пармезан, чеснок, соль и перец", "Смешать все ингредиенты", "Подавать горячим" },
                    ImagePath = ""
                });

                Manager.AddRecipe(new Recipe
                {
                    Name = "Греческий салат",
                    Type = "Салат",
                    Cuisine = "Греческая",
                    Ingredients = new ObservableCollection<string> { "Помидоры", "Огурцы", "Фета", "Маслины", "Лук", "Оливковое масло", "Соль", "Перец" },
                    Instructions = new ObservableCollection<string> { "Нарезать помидоры, огурцы и лук", "Добавить фету и маслины", "Заправить оливковым маслом, солью и перцем", "Подавать охлажденным" },
                    ImagePath = ""
                });

                Manager.AddRecipe(new Recipe
                {
                    Name = "Спагетти с томатным соусом",
                    Type = "Макароны",
                    Cuisine = "Итальянская",
                    Ingredients = new ObservableCollection<string> { "Спагетти", "Помидоры", "Лук", "Чеснок", "Оливковое масло", "Базилик", "Соль", "Перец" },
                    Instructions = new ObservableCollection<string> { "Подготовить томаты, лук и чеснок", "Обжарить лук и чеснок в оливковом масле", "Добавить нарезанные помидоры и базилик", "Варить соус до загустения", "Приготовить спагетти", "Подавать горячим" },
                    ImagePath = ""
                });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            recipeListBox.SelectionChanged += recipeListBox_SelectionChanged;
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            // Получение экземпляра MainViewModel из DataContext
            MainViewModel viewModel = (MainViewModel)DataContext;

            // Получение данных из текстовых полей и списков
            string name = nameTextBox.Text;
            string type = typeTextBox.Text;
            string cuisine = cuisineTextBox.Text;
            ObservableCollection<string> ingredients = new ObservableCollection<string>(ingredientsListBox.Items.OfType<string>());
            ObservableCollection<string> instructions = new ObservableCollection<string>(instructionsListBox.Items.OfType<string>());
            string imagePath = imagePathTextBox.Text;

            // Проверка наличия обязательных полей
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(cuisine))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля: Название, Тип, Кухня", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создание нового объекта рецепта
            Recipe newRecipe = new Recipe
            {
                Name = name,
                Type = type,
                Cuisine = cuisine,
                Ingredients = ingredients,
                Instructions = instructions,
                ImagePath = imagePath
            };

            // Добавление рецепта в менеджер рецептов
            viewModel.Manager.AddRecipe(newRecipe);

            // Очистка полей после добавления рецепта
            ClearRecipeFields();

            // Обновление отображаемого списка рецептов
            recipeListBox.ItemsSource = viewModel.Manager.Recipes;
        }

        private void RemoveRecipe_Click(object sender, RoutedEventArgs e)
        {
            // Получение экземпляра MainViewModel из DataContext
            MainViewModel viewModel = (MainViewModel)DataContext;

            // Проверка, выбран ли какой-то рецепт
            if (recipeListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите рецепт для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получение выбранного рецепта
            Recipe selectedRecipe = (Recipe)recipeListBox.SelectedItem;

            // Подтверждение удаления с использованием диалогового окна
            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить рецепт '{selectedRecipe.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // Если пользователь подтвердил удаление, удаляем рецепт
            if (result == MessageBoxResult.Yes)
            {
                viewModel.Manager.RemoveRecipe(selectedRecipe);

                // Обновление отображаемого списка рецептов
                recipeListBox.ItemsSource = viewModel.Manager.Recipes;
            }
        }

        private void ExportToDoc_Click(object sender, RoutedEventArgs e)
        {
            // Получение экземпляра MainViewModel из DataContext
            MainViewModel viewModel = (MainViewModel)DataContext;

            // Проверка, выбран ли какой-то рецепт
            if (recipeListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите рецепт для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Получение выбранного рецепта
            Recipe selectedRecipe = (Recipe)recipeListBox.SelectedItem;

            try
            {
                // Создание документа
                using (var doc = DocX.Create(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\RecipeDocument.docx"))
                {
                    // Добавление информации о рецепте в документ
                    doc.InsertParagraph("Рецепт: " + selectedRecipe.Name).FontSize(18).Bold().Alignment = Alignment.center;
                    doc.InsertParagraph("Тип: " + selectedRecipe.Type);
                    doc.InsertParagraph("Кухня: " + selectedRecipe.Cuisine);
                    doc.InsertParagraph("Ингредиенты:").Bold();
                    foreach (var ingredient in selectedRecipe.Ingredients)
                    {
                        doc.InsertParagraph("- " + ingredient);
                    }
                    doc.InsertParagraph("Инструкции:").Bold();
                    foreach (var instruction in selectedRecipe.Instructions)
                    {
                        doc.InsertParagraph(instruction);
                    }

                    // Вставка изображения
                    if (!string.IsNullOrEmpty(selectedRecipe.ImagePath))
                    {
                        var image = doc.AddImage(Path.GetFullPath(@"..\..\") + selectedRecipe.ImagePath);
                        var picture = image.CreatePicture();
                        doc.InsertParagraph().AppendPicture(picture).Alignment = Alignment.center;
                    }

                    // Сохранение документа
                    doc.Save();

                    MessageBox.Show($"Рецепт '{selectedRecipe.Name}' успешно экспортирован в {Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\RecipeDocument.docx"}", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при экспорте рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            // Получение значения из TextBox для ингредиента
            string newIngredient = ingredientTextBox.Text.Trim();

            // Проверка наличия текста в TextBox
            if (string.IsNullOrWhiteSpace(newIngredient))
            {
                MessageBox.Show("Введите название ингредиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление ингредиента в список
            ingredientsListBox.Items.Add(newIngredient);

            // Очистка TextBox после добавления
            ingredientTextBox.Clear();
        }

        private void AddInstruction_Click(object sender, RoutedEventArgs e)
        {
            // Получение значения из TextBox для инструкции
            string newInstruction = instructionTextBox.Text.Trim();

            // Проверка наличия текста в TextBox
            if (string.IsNullOrWhiteSpace(newInstruction))
            {
                MessageBox.Show("Введите текст инструкции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Добавление инструкции в список
            instructionsListBox.Items.Add(newInstruction);

            // Очистка TextBox после добавления
            instructionTextBox.Clear();
        }

        private void SearchRecipes_Click(object sender, RoutedEventArgs e)
        {
            // Получение экземпляра MainViewModel из DataContext
            MainViewModel viewModel = (MainViewModel)DataContext;

            // Получение значения из TextBox для поиска
            string searchKeyword = searchTextBox.Text.Trim().ToLower();

            // Проверка наличия текста в TextBox
            if (string.IsNullOrWhiteSpace(searchKeyword))
            {
                MessageBox.Show("Введите ключевое слово для поиска", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Поиск рецептов по ключевому слову
            ObservableCollection<Recipe> searchResults = viewModel.Manager.SearchRecipes(searchKeyword);

            // Обновление отображаемого списка рецептов
            recipeListBox.ItemsSource = searchResults;

            // Очистка TextBox после поиска
            searchTextBox.Clear();
        }

        private void ClearSearchResults_Click(object sender, RoutedEventArgs e)
        {
            // Получение экземпляра MainViewModel из DataContext
            MainViewModel viewModel = (MainViewModel)DataContext;

            // Очистка результатов поиска и обновление отображаемого списка рецептов
            viewModel.ClearSearchResults(recipeListBox);
        }

        private void ClearRecipeFields()
        {
            // Очистка всех текстовых полей и списков
            nameTextBox.Clear();
            typeTextBox.Clear();
            cuisineTextBox.Clear();
            ingredientsListBox.Items.Clear();
            instructionsListBox.Items.Clear();
            imagePathTextBox.Clear();
        }

        private void recipeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение экземпляра MainViewModel из DataContext
            MainViewModel viewModel = (MainViewModel)DataContext;

            // Обновление SelectedRecipe при изменении выбора в recipeListBox
            viewModel.SelectedRecipe = (Recipe)recipeListBox.SelectedItem;
        }
    }
}
