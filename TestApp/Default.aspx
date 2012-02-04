<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="TestApp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to the ClaimsProxy test application</h2>
    <p>
        The ClaimsProxy library enables you to get a WIF cookie collection for a 
        SharePoint site which is protected by Claims Based Authentication.&nbsp; It 
        assumes that ADFS is configured as the Trusted Identity Token Issuer and that 
        the down-stream identity provider is based on the StarterSTS / IdentityServer 
        project.</p>
    <p>
        Todo:</p>
    <p>
        - Add support for multiple WIF cookies (FedAuthN)<br />
        - Implement Tracing to replace the DebugEvent delegate</p>
    <p>
        Any application utilising the library should implement some caching of the 
        SharePoint cookie for the duration of it&#39;s lifetime against the requesting 
        username to reduce authentication round-trips.</p>
    <p>
        <strong>Configuration</strong></p>
    <table>
        <tbody>
            <tr>
                <td>StarterSts Endpoint</td>
                <td><asp:TextBox runat="server" ID="txtStarterStsEndpoint" Width="550px" >https://startersts.yourdomain.com/startersts/users/issue.svc/mixed/username</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>StarterSts Username</td>
                <td>
                    <asp:TextBox ID="txtStarterStsUsername" runat="server" Width="250px">user@yourdomain.com</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>StarterSts Password</td>
                <td>
                    <asp:TextBox ID="txtStarterStsPassword" runat="server" Width="250px">password</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>StarerSts ADFS Realm</td>
                <td>
                    <asp:TextBox ID="txtStarterStsADFSRealm" runat="server" Width="550px">https://adfs.yourdomain.com/adfs/services/trust/13/issuedtokenmixedsymmetricbasic256</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>ADFS Endpoint</td>
                <td>
                    <asp:TextBox ID="txtADFSEndpoint" runat="server" Width="550px">https://adfs.yourdomain.com/adfs/services/trust/13/issuedtokenmixedsymmetricbasic256</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>SharePoint Realm</td>
                <td>
                    <asp:TextBox ID="txtSPRealm" runat="server" Width="550px">urn:sharepoint:sp.yourdomain.com</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>SharePoint Site</td>
                <td>
                    <asp:TextBox ID="txtSPSite" runat="server" Width="550px">https://sp.yourdomain.com/</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Ignore SSL Checking?</td>
                <td>
                    <asp:CheckBox ID="chkIgnoreSSL" runat="server" Checked="True" />
                </td>
            </tr>
        </tbody>
    </table>
    <p>
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" 
            Text="GetGroupCollectionFromSite" />
    </p>
    <p>
        <strong>Debug</strong></p>
    <p><asp:TextBox ID="txtDebug" runat="server" TextMode="MultiLine" Width="685px" 
            Height="100px"></asp:TextBox></p>
            
        <p>
        <strong>Output</strong></p>

    <p><asp:TextBox ID="txtOutput" runat="server" TextMode="MultiLine" Width="685px" 
                        Height="100px"></asp:TextBox></p>
    <p>
        &nbsp;</p>
</asp:Content>
