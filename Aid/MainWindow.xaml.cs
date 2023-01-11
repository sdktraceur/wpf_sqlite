using Aid.Contexts.AidContext;
using Aid.Models.Aid;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AidContext _ctx;
        private int _conditionMax = 0;
        private int _conditionActual = 0;

        string[] militaryMachineNames = new string[] {
            "Tempest",
            "Vindicator",
            "Thunderbolt",
            "Valkyrie",
            "Raptor",
            "Goliath",
            "Aegis",
            "Predator",
            "Eagle Eye",
            "Chronos",
            "Nightwing",
            "Sky Guardian",
            "Firestorm",
            "Silverback",
            "Avenger"
        };

        string[] generation = new string[] {
            "I",
            "II",
            "III",
            "IV",
            "V",
            "VI",
            "VI",
            "VII",
            "VIII",
            "IX",
            "X",
            "XI",
            "XII"
        };

        private string GenerateNewName() =>
            $"{GetRandomName()} {GetRandomGeneration()}";

        private string GetRandomGeneration() =>
            generation[GetRandomIndex(generation.Length - 1)];

        private string GetRandomName() =>
            militaryMachineNames[GetRandomIndex(militaryMachineNames.Length - 1)];

        private string GetRandomType() =>
            militaryMachines[GetRandomIndex(militaryMachines.Length - 1)];

        private int GetRandomIndex(int maxIndex)
        {
            Random rnd = new Random();
            return rnd.Next(0, maxIndex);
        }

        string[] militaryMachines = new string[] {
            "Combat vehicles",
            "Artillery",
            "Aircraft",
            "Warships",
            "Support vehicles",
            "Electronic warfare systems",
            "Bombs and other explosives",
            "Explosive Ordnance Disposal robot"
        };
        public MainWindow()
        {
            _ctx = new AidContext();      
            _ctx.Database.EnsureCreated();
            InitializeComponent();
            
            TypeList.ItemsSource = militaryMachines.ToList();
            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            var items = GetEquipment();
            dbView.ItemsSource = items;
            _conditionMax = items.Sum(x => x.Strength) ?? 0;
            _conditionActual = items.Sum(x => x.StrengthLeft) ?? 0;
            EquipStatus.Content = $"Ttl: {_conditionMax} / Left: {_conditionActual}";
        }

        private List<Equipment> GetEquipment() =>
            _ctx.Equipment.OrderByDescending(x => x.Id).ToList();

        private void GetReportBtn_Click(object sender, RoutedEventArgs e)
        {         

            Random rnd = new Random();
            var roundDamage =  rnd.Next(1, 10);

            var _all = GetEquipment();

            foreach (var item in _all)
            {
                item.StrengthLeft -= roundDamage;
            }

            var sumDamage = roundDamage * _all.Count;
            var destroyed = _all.Where(x => x.StrengthLeft <= 0);
            var _dCount = destroyed.Count();

            if(sumDamage > 0)
            {
                string messageBoxText = $"Orks from Ruzzland launched: {sumDamage} missiles!";
                string messageDetails = _dCount > 0 ? $"They destroyed {_dCount} pieces of Ukrainian equipment!" : "Ukraine was bombed all day, we didn't lose any equipment, but some was damaged!";
                string caption = "ZSU reporting!";
                MessageBox.Show($"{messageBoxText} {messageDetails}", caption);
            }
            else
            {
                string caption = "ZSU reporting!";
                MessageBox.Show($"This time, we destroyed all missiles!", caption);
            }

            _ctx.RemoveRange(destroyed);
            _ctx.SaveChanges();

            if(GetEquipment().Count < 2 )
            {
                MessageBox.Show($"“In the end, it is impossible not to become what others believe you are.”\r\n― Julius Caesar \r\n Ukrainians by them slef just buy extra equipment, we will never lose!!!");
                Random _rnd = new Random();

                for (int i = 0; i < _rnd.Next(7, 24); i++)
                {
                    Random __rnd = new Random();

                    var _newEquipment = new Equipment();

                    _newEquipment.Name = GenerateNewName();
                    _newEquipment.Type = GetRandomType();
                    _newEquipment.Strength = __rnd.Next(6, 10);
                    _newEquipment.StrengthLeft = _newEquipment.Strength;
                    _newEquipment.Date = DateTime.Now;

                    _ctx.Equipment.Add(_newEquipment);                
                }
                _ctx.SaveChanges();
                UpdateDataGrid();
            }

            UpdateDataGrid();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            AddEquipment();
        }

        private void AddEquipment()
        {
            var _nameValid = EquipName.Text is not null && EquipName.Text is not "";
            var _typeValid = TypeList.SelectedValue is not null && TypeList.SelectedValue is not "";

            if (_nameValid && _typeValid)
            {
                Random rnd = new Random();

                var _newEquipment = new Equipment();

                _newEquipment.Name = EquipName.Text;
                _newEquipment.Type = (string)(TypeList.SelectedValue!);
                _newEquipment.Strength = rnd.Next(1, 10);
                _newEquipment.StrengthLeft = _newEquipment.Strength;
                _newEquipment.Date = DateTime.Now;

                _ctx.Equipment.Add(_newEquipment);
                _ctx.SaveChanges();
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show($"Generate or type name and choose one of suggested type!", "Form not valid!");
            }
            EquipName.Text = null;
            TypeList.SelectedValue = null;
        }

        private void GenerateValues()
        {
            EquipName.Text = GenerateNewName();
            TypeList.SelectedValue = GetRandomType();
        }

        private void GenNameBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateValues();
        }
    }
}
