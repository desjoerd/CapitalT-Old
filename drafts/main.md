# Main Draft

I like the way Orchard does things ;-). One of the nicest things is the way localization is handled.

### CSHARP
Translate messages by using a Localizer (a la Orchard).
```csharp
public delegate LocalizedString Localizer(string text, params object[] args);

public static class LocalizerExtensions {
    public static LocalizedString Plural(this Localizer T, string textSingular, string textPlural, int count, params object[] args) {
        return T(count == 1 ? textSingular : textPlural, new object[] { count }.Concat(args).ToArray());
    }

    public static LocalizedString Plural(this Localizer T, string textNone, string textSingular, string textPlural, int count, params object[] args) {
        switch (count) {
            case 0:
                return T(textNone, new object[] {count}.Concat(args).ToArray());
            case 1:
                return T(textSingular, new object[] {count}.Concat(args).ToArray());
            default:
                return T(textPlural, new object[] {count}.Concat(args).ToArray());
        }
    }
}

public Localizer T { get; set; }

public LocalizedString GetLocalizedString() {
    return T("hello world!");
}

public string GetLocalizedText() {
    return T("hello world!").Text;
}
```

### Javascript
I want to use the localization Api of CSHARP also in JS. This gives a nice standard way of localization.
```js
    var localizedText = T("Hello World"); // Don't know to return a "LocalizedString" class or a normal string.

    // also for plurals:
    var localizedPlural = T.plural("Hello there is one world", "Hello there are {0} worlds", 2); // camelcase or pascalcase?
```

### Html
For html it's a bit harder. I would als like to localize html (a la i18next). i18next provides a api with data-attributes.
```html
<p data-T>Hello content</p>
<!-- or -->
<p data-T="Hello content"></p>

<!-- for other attributes -->
<a href="#" data-T="[title]Hello title;Hello content"></a> 
<!-- [] and ; can create a problem. A solution would be using html encoding -->
```