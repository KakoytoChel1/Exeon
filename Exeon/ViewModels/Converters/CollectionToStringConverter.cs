using Exeon.Models.Commands;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Exeon.ViewModels.Converters
{
    internal class CollectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is ObservableCollection<TriggerCommand> collection)
            {
                StringBuilder resultBuilder = new StringBuilder();

                if(collection.Count > 0)
                {
                    foreach (var item in collection)
                    {
                        resultBuilder.Append($"{item.CommandText}; ");
                    }

                    return resultBuilder.ToString();
                }

                return "Nothing found :(";
            }

            return "Indentify error!";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
