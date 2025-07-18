using System.Collections;
using System.Collections.Specialized;

namespace Ona.Main.Controls;

public partial class ListViewLite : ContentView
{
    public ListViewLite()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ListViewLite), null, propertyChanged: OnItemsSourceChanged);

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ListViewLite), propertyChanged: OnItemTemplateChanged);

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly BindableProperty HorizontalScrollBarVisibilityProperty =
    BindableProperty.Create(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(ListViewLite));

    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    public static readonly BindableProperty VerticalScrollBarVisibilityProperty =
        BindableProperty.Create(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(ListViewLite));

    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    public double ContentWidth
        => ItemsPanel.Width;

    public double ContentHeight
        => ItemsPanel.Height;

    public double ScrollX
        => ItemsScrollView.ScrollX;

    public double ScrollY
        => ItemsScrollView.ScrollY;

    public async Task ScrollToIndexAsync(int index, ScrollToPosition position, bool animated)
        => await ItemsScrollView.ScrollToAsync((Element)ItemsPanel.Children[index], position, animated);

    public async Task ScrollToOffsetAsync(double offset, bool animated)
        => await ItemsScrollView.ScrollToAsync(0, offset, animated);

    public event EventHandler<ScrolledEventArgs>? Scrolled;

    public event EventHandler<ListItemAddedEventArgs>? ItemAdded;

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        => ((ListViewLite)bindable).OnItemsSourceChanged(oldValue, newValue);

    private void OnItemsSourceChanged(object oldValue, object newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotifyCollectionChanged)
            oldNotifyCollectionChanged.CollectionChanged -= NewNotifyCollectionChanged_CollectionChanged;

        InitializeItems();

        if (newValue is INotifyCollectionChanged newNotifyCollectionChanged)
            newNotifyCollectionChanged.CollectionChanged += NewNotifyCollectionChanged_CollectionChanged;
    }

    private void NewNotifyCollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                for (int i = 0; i < e.NewItems!.Count; i++)
                {
                    var newView = CreateItemView(e.NewItems[i]!);
                    var index = e.NewStartingIndex + i;
                    ItemsPanel.Insert(index, newView);
                    ItemAdded?.Invoke(this, new ListItemAddedEventArgs(index, newView));
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                for (int i = 0; i < e.OldItems!.Count; i++)
                    ItemsPanel.RemoveAt(e.OldStartingIndex);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void InitializeItems()
    {
        ItemsPanel.Clear();

        if (ItemsSource == null || ItemTemplate == null)
            return;

        foreach (var item in ItemsSource)
            ItemsPanel.Add(CreateItemView(item));
    }

    private IView CreateItemView(object item)
    {
        var template = ItemTemplate is DataTemplateSelector selector
            ? selector.SelectTemplate(item, this)
            : ItemTemplate;

        var content = (BindableObject)template.CreateContent();
        content.BindingContext = item;
        return (IView)content;
    }

    private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        => ((ListViewLite)bindable).InitializeItems();

    private void ItemsScrollView_Scrolled(object sender, ScrolledEventArgs e)
        => Scrolled?.Invoke(this, e);
}