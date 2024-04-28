using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus;

public class AlueIdToNimiConverter : IValueConverter
{
    private readonly ObservableCollection<Alue> _alueCollection;

    public AlueIdToNimiConverter(ObservableCollection<Alue> alueCollection)
    {
        _alueCollection = alueCollection;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null)
        {
            string alueId = value.ToString();
            var alue = _alueCollection.FirstOrDefault(a => a.alue_id == alueId);
            return alue?.nimi;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
