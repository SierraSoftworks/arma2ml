using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;
using BISLib;

namespace ArmALauncher
{
    public class PinnedConfiguration : DependencyObject
    {
        public PinnedConfiguration(string name, LaunchParameters parameters, Color? gameColour = null)
        {
            Name = name;
            Parameters = parameters;

            if (gameColour.HasValue)
                GameColour = new SolidColorBrush(gameColour.Value);
            else
                switch(Parameters.Game.Name)
                {
                    case "ArmA2":
                        GameColour = new SolidColorBrush(Color.FromRgb(0,0,0));
                        break;
                    case "Operation Arrowhead":
                        GameColour = new SolidColorBrush(Color.FromRgb(40,40,40));
                        break;
                    case "Combined Operations":
                        GameColour = new SolidColorBrush(Color.FromRgb(80,80,80));
                        break;
                }
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(PinnedConfiguration));

        public static readonly DependencyProperty GameColourProperty = DependencyProperty.Register("GameColour", typeof(Brush), typeof(PinnedConfiguration));

        public string Name
        { 
            get { return (string)GetValue(NameProperty); }
            set
            {
                var oldValue = Name;
                SetValue(NameProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(NameProperty, oldValue, value));
            }
        }

        public Brush GameColour
        {
            get { return (Brush)GetValue(GameColourProperty); }
            set
            {
                var oldValue = GameColour;
                SetValue(GameColourProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(GameColourProperty, oldValue, value));
            }
        }

        public LaunchParameters Parameters
        { get; set; }
    }
}
