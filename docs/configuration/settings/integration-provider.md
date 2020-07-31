# Integration Provider Setting

<table>
    <tr>
        <td><b>Setting</b></td>
        <td><code>changelog:integrations:provider</code></td>
    </tr>
    <tr>
        <td><b>Environment Variable</b></td>
        <td><code>CHANGELOG__INTEGRATIONS__PROVIDER</code></td>
    </tr>
    <tr>
        <td><b>Commandline Parameter</b></td>
        <td>-</td>
    </tr>
    <tr>
        <td><b>Default value</b></td>
        <td><code>none</code></td>
    </tr>
    <tr>
        <td><b>Allowed values</b></td>
        <td>
            <ul>
                <li><code>none</code></li>
                <li><code>GitHub</code></li>
                <li><code>GitLab</code></li>
            </ul>
        </td>
    </tr>
    <tr>
        <td><b>Version Support</b></td>
        <td>0.1+</td>
    </tr>
</table>

Sets the *Integration Provider* to use.

For details on see [Integrations](../../integrations.md).

## Example

Enable the GitHub integration provider:

```json
{
    "changelog" : {
        "integrations" :{
            "provider" : "github"
        }
    }
}
```

## See Also

- [Configuration Overview](../../configuration.md)