<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
</head>
<body>
    <% using (Html.BeginForm()) { %>
    <table>
        <tr>
            <th colspan="2">
                Login credentials:
            </th>
        </tr>
        <tr>
            <td style="text-align: right">
                <label for="username">
                    Username:</label>
            </td>
            <td >
                <input type="text" name="username" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                <label for="password">
                    Password:</label>
            </td>
            <td >
                <input type="password" name="password" />
            </td>
        </tr>
        <tr style="text-align: center">
            <td colspan="2" >
                <input type="submit" value="Log On" />
            </td>
        </tr>
    </table>
    <div style="text-align: center">
        <h2>
            <%= Html.Encode(ViewData["ErrorDetails"]) %></h2>
    </div>
    <% } %>
</body>
</html>
