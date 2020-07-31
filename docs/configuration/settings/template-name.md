# Template Name Setting

<table>
    <tr>
        <td><b>Setting</b></td>
        <td><code>changelog:template:name</code></td>
    </tr>
    <tr>
        <td><b>Environment Variable</b></td>
        <td><code>CHANGELOG__TEMPLATE__NAME</code></td>
    </tr>
    <tr>
        <td><b>Commandline Parameter</b></td>
        <td><code>template</code></td>
    </tr>
    <tr>
        <td><b>Default value</b></td>
        <td><code>default</code></td>
    </tr>
    <tr>
        <td><b>Allowed values</b></td>
        <td>
            <ul>
                <li><code>Default</code></li>
                <li><code>GitHubRelease</code></li>
                <li><code>GitLabRelease</code></li>
            </ul>
        </td>
    </tr>
    <tr>
        <td><b>Version Support</b></td>
        <td>0.2+</td>
    </tr>
</table>

Sets the template to use for generating the change log.
For details see [Templates](../../templates.md).

## Example

```json
{
    "changelog" : {
        "template" : {
            "name": "default"
        }
    }
}
```

## See Also

- [Configuration Overview](../../configuration.md)