<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/RMS.Master" CodeBehind="RecordProcessing.aspx.vb" Inherits="SIMS.RecordProcessing" %>
<%@ MasterType  virtualPath="~/RMS.master" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxtoolkit" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="c1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="c2" ContentPlaceHolderID="cph1" Runat="Server">
<asp:Panel ID="pnlHasAccess" runat="server">
    <telerik:RadAjaxPanel ID="rapMain" runat="server">
        <table>
            <tr>
                <td>
                    <fieldset style="height:100px;width:350px;">
                        <legend>Explanation</legend>
                        <div style="padding:5px;">
                        <label>Click on the record-type description to <asp:Label ID="lblListType" runat="server" /> the record.</label><br />
                        <label>Click on the site number to open the Station Information page.</label><br />
                        <img border="0" src="images/lock.png" alt="lock" style="padding-right:10px;padding-left:5px;" />
                        <label>Site currently locked</label><br />
                        <img border="0" src="images/save_icon.gif" alt="updates pending" 
                            style="padding-right:12px;padding-left:5px;" />
                        <label>Updates pending</label>
                        </div>
                    </fieldset>
                </td>
                <td>
                    <fieldset style="height:100px;width:250px;">
                        <legend>Quick Links</legend>
                        <div style="padding:5px;">
                            <asp:Literal ID="ltlQuickLinks" runat="server" />
                        </div>
                    </fieldset>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hfListType" runat="server" />
        <asp:PlaceHolder ID="phRecordsToProcess" runat="server"></asp:PlaceHolder>
    </telerik:RadAjaxPanel>
</asp:Panel>
</asp:Content>
