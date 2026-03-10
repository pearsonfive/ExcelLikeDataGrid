using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelLikeDataGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Person> People { get; set; }
        public List<Person> People2 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadMockData();
            DataContext = this;
        }

        private void LoadMockData()
        {
            People = new List<Person>
            {
                new Person { Name = "Alice", Age = 30, Department = "Engineering", Country = "USA" },
                new Person { Name = "Bob", Age = 25, Department = "Sales", Country = "Canada" },
                new Person { Name = "Charlie", Age = 35, Department = "Engineering", Country = "UK" },
                new Person { Name = "Diana", Age = 28, Department = "HR", Country = "USA" },
                new Person { Name = "Edward", Age = 45, Department = "Management", Country = "UK" },
                new Person { Name = "Fiona", Age = 32, Department = "Sales", Country = "Australia" },
                new Person { Name = "George", Age = 29, Department = "Engineering", Country = "Canada" },
                new Person { Name = "Hannah", Age = 42, Department = "HR", Country = "Australia" }
            };

            People2 = new List<Person>
            {
                new Person { Name = "Alice", Age = 30, Department = "Engineering", Country = "USA" },
                new Person { Name = "Bob", Age = 25, Department = "Sales", Country = "Canada" },
                new Person { Name = "Charlie", Age = 35, Department = "Engineering", Country = "UK" },
                new Person { Name = "Diana", Age = 28, Department = "HR", Country = "USA" },
                new Person { Name = "Edward", Age = 45, Department = "Management", Country = "UK" },
                new Person { Name = "Fiona", Age = 32, Department = "Sales", Country = "Australia" },
                new Person { Name = "George", Age = 29, Department = "Engineering", Country = "Canada" },
                new Person { Name = "Hannah", Age = 42, Department = "HR", Country = "Australia" }
            };
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Department { get; set; }
        public string Country { get; set; }
    }
}
