using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.Navigation
{
    public class PageStackEntry : DependencyObject
    {
        #region DependencyProperties
        public static readonly DependencyProperty SourcePageTypeProperty =
            DependencyProperty.Register("SourcePageType", typeof(Type), typeof(PageStackEntry), null);
        public static readonly DependencyProperty MasterPageTypeProperty =
            DependencyProperty.Register("MasterPageType", typeof(Type), typeof(PageStackEntry), null);
        #endregion

        #region Properties
        public Type SourcePageType
        {
            get { return (Type)this.GetValue(SourcePageTypeProperty); }
            private set { this.SetValue(SourcePageTypeProperty, value); }
        }
        public Type MasterPageType
        {
            get { return (Type)this.GetValue(MasterPageTypeProperty); }
            private set { this.SetValue(MasterPageTypeProperty, value); }
        }

        public object Parameter
        {
            get;
            private set;
        }

        #endregion

        #region MethodsPublic
        /// <summary>
        /// Initializes a new instance of the <see cref="PageStackEntry"/> class.
        /// </summary>
        /// <param name="sourcePageType">The type of page associated with the navigation entry.</param>
        /// <param name="masterPageType">The type of master page associated with the navigation entry.</param>
        /// <param name="parameter">The navigation parameter associated with the navigation entry.</param>
        public PageStackEntry(Type sourcePageType, Type masterPageType, object parameter)
        {
            this.SourcePageType = sourcePageType;
            this.MasterPageType = masterPageType;
            this.Parameter = parameter;
        }
        #endregion
    }
}
