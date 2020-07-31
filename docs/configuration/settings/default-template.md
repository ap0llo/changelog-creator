# Default Template Settings

## Markdown Preset

<table>
    <tr>
        <td><b>Setting</b></td>
        <td><code>changelog:template:default:markdownPreset</code></td>
    </tr>
    <tr>
        <td><b>Environment Variable</b></td>
        <td><code>CHANGELOG__TEMPLATE__DEFAULT__MARKDOWNPRESET</code></td>
    </tr>
    <tr>
        <td><b>Commandline Parameter</b></td>
        <td>-</td>
    </tr>
    <tr>
        <td><b>Default value</b></td>
        <td><code>default</code></td>
    </tr>
    <tr>
        <td><b>Allowed values</b></td>
        <td>
            <ul>
                <li><code>default</code></li>
                <li><code>MkDocs</code></li>
            </ul>
        </td>
    </tr>
    <tr>
        <td><b>Version Support</b></td>
        <td>0.2+</td>
    </tr>
</table>

The *Markdown Preset (Default Template)* customizes serialization of Markdown for the default template (see [Template Name setting](./template-name.md)).

**Note:** This setting has no effect when a template other than `default` is used.

Supported values are:

- `default`: Produces Markdown that should work in most environments, including GitHub and GitLab
- `MkDocs`: Produces Markdown optimized for being rendered by Python-Markdown and [MkDocs](https://www.mkdocs.org/)

For details on the differences between the presets, see also [Markdown Generator docs](https://github.com/ap0llo/markdown-generator/blob/master/docs/apireference/Grynwald/MarkdownGenerator/MdSerializationOptions/Presets/index.md).

### Example

```json
{
    "changelog" : {
        "template" : {
            "default": {
                "markdownPreset" : "MkDocs"
            }
        }
    }
}
```

## See Also

- [Configuration Overview](../../configuration.md)