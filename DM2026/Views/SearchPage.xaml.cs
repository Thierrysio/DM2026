using DantecMarketApp.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Input;

namespace DantecMarketApp.Views
{
    public partial class SearchPage : ContentPage
    {
        private List<Category> _parentCategories;
        private Category _selectedParentCategory;
        private List<Category> _subCategories;
        private ObservableCollection<Product> _searchResults;
        private readonly HttpClient _httpClient;
        private string _searchQuery = string.Empty;
        private bool _isLoading = false;
        
        public List<Category> ParentCategories 
        { 
            get => _parentCategories; 
            set 
            { 
                _parentCategories = value;
                OnPropertyChanged();
            } 
        }
        
        public Category SelectedParentCategory
        {
            get => _selectedParentCategory;
            set
            {
                _selectedParentCategory = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadSubCategories(value);
                }
            }
        }
        
        public List<Category> SubCategories
        {
            get => _subCategories;
            set
            {
                _subCategories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Product> SearchResults
        {
            get => _searchResults;
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }


        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand SearchCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand ClearSearchCommand { get; }


        public SearchPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://213.130.144.159") };
            SearchResults = new ObservableCollection<Product>();
            SubCategories = new List<Category>();
            
            SearchCommand = new Command(async () => await PerformSearch());
            SelectCategoryCommand = new Command<Category>(async (category) => await LoadProductsByCategory(category));

            ClearSearchCommand = new Command(() =>
            {
                SearchQuery = string.Empty;
            });

            BindingContext = this;
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                IsLoading = true;
                var response = await _httpClient.GetAsync("/api/mobile/allcategoriesParent");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    ParentCategories = JsonSerializer.Deserialize<List<Category>>(content, options);
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible de charger les catégories", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }


        private void LoadSubCategories(Category parentCategory)
        {
            SubCategories = parentCategory.LesCategories ?? new List<Category>();
        }

        private async Task LoadProductsByCategory(Category category)
        {
            try
            {
                IsLoading = true;
                SearchResults.Clear(); // Vider la collection existante

                var parentCategory = ParentCategories?.FirstOrDefault(p =>
                    p.LesCategories?.Any(c => c.Id == category.Id) ?? false);

                if (parentCategory != null)
                {
                    var subCategory = parentCategory.LesCategories?.FirstOrDefault(c => c.Id == category.Id);
                    if (subCategory?.LesProduits != null)
                    {
                        foreach (var product in subCategory.LesProduits)
                        {
                            SearchResults.Add(product);
                        }
                    }
                }

                if (SearchResults.Count == 0)
                {
                    await DisplayAlert("Info", "Aucun produit dans cette catégorie", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite: {ex.Message}", "OK");
                SearchResults = new ObservableCollection<Product>();
            }
            finally
            {
                IsLoading = false;
            }
        }


        private async Task PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 2)
            {
                await DisplayAlert("Info", "Veuillez saisir au moins 2 caractères", "OK");
                return;
            }

            try
            {
                IsLoading = true;
                SearchResults.Clear(); // Vider la collection existante

                // Recherche dans toutes les catégories et sous-catégories
                if (ParentCategories != null)
                {
                    foreach (var parentCategory in ParentCategories)
                    {
                        if (parentCategory.LesCategories != null)
                        {
                            foreach (var subCategory in parentCategory.LesCategories)
                            {
                                if (subCategory.LesProduits != null)
                                {
                                    var matchingProducts = subCategory.LesProduits
                                        .Where(p => p.NomProduit.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                  p.DescriptionCourte.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                                        .ToList();

                                    // Ajouter chaque produit individuellement à l'ObservableCollection
                                    foreach (var product in matchingProducts)
                                    {
                                        SearchResults.Add(product);
                                    }
                                }
                            }
                        }
                    }
                }

                if (SearchResults.Count == 0)
                {
                    await DisplayAlert("Info", "Aucun produit ne correspond à votre recherche", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite: {ex.Message}", "OK");
                SearchResults.Clear();
            }
            finally
            {
                IsLoading = false;
            }
        }



        private async void OnProductSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Product product)
            {
                // Reset selection
                ((ListView)sender).SelectedItem = null;
                
                // Navigate to product detail page
                await Navigation.PushAsync(new ProductDetailPage(product));
                //await DisplayAlert("Produit sélectionné", $"Vous avez sélectionné: {product.NomProduit}", "OK");
            }
        }
    }
}