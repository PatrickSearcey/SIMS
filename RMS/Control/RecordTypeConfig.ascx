<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecordTypeConfig.ascx.cs" Inherits="RMS.Control.RecordTypeConfig"%>
<style type="text/css">
    .validator {
        color:red;
        font-weight:bold;
    }
    .EditFormHeader {
        line-height:12pt;
        font-size:12pt;
        font-weight:bold;
        color:#b84626;
    }
    .EditFormTable {
        width:750px;
        font-size:10pt;
        padding:10px;
    }
</style>
<table id="Table2" cellspacing="2" cellpadding="1" width="750" rules="none" border="0" class="EditFormTable">
    <tr class="EditFormHeader">
        <td colspan="2">
            <asp:Label ID="lblHeading" runat="server" /><br /><br />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table id="Table3" cellspacing="1" cellpadding="1" width="600" border="0">
                <tr>
                    <td>Record-type code:</td>
                    <td nowrap colspan="2"><asp:TextBox id="tbCode" runat="server" Text='<%# DataBinder.Eval( Container, "DataItem.type_cd" ) %>' />
                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="tbCode" Text="You must enter a code" CssClass="validator" />
                    </td>
                </tr>
                <tr>
                    <td nowrap>Record-type description:</td>
                    <td nowrap colspan="2"><asp:TextBox id="tbDescription" runat="server" Text='<%# DataBinder.Eval( Container, "DataItem.type_ds") %>' Columns="50" tabIndex="1" />
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" CssClass="validator"
                        Text="You must enter a description" ControlToValidate="tbDescription"/>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblContorNoncont" runat="server" /></td>
                    <td><asp:Label ID="lblCONStatus" runat="server" />
                        <asp:RadioButtonList ID="rblContorNoncont" runat="server" TabIndex="2" >
                            <asp:ListItem Value="cont">Time-series</asp:ListItem>
                            <asp:ListItem Value="noncont">Non-time-series</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rfvContorNoncont" runat="server" CssClass="validator"
                            Text="You must select one" ControlToValidate="rblContorNoncont" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2"><b>Choose the template to use when analyzing these records:</b></td>
    </tr>
    <tr>
        <td valign="top">
            <telerik:RadDropDownList ID="rddlTemplates" runat="server" DataValueField="TemplateID" DataTextField="TemplateName" Skin="Bootstrap" Width="300px">
            </telerik:RadDropDownList><br />
            <asp:Label ID="lblRequired" runat="server" CssClass="validator" Text="You must select a template!" />
        </td>
        <td><b>Click the links to view the available templates:</b><br />
            <asp:DataList ID="dlTemplates" runat="server">
                <ItemTemplate>
                    <a href='<%# "../Handler/TemplateViewer.ashx?type=analyze&TemplateID=" + Eval("TemplateID") %>' target="_blank"><%# Eval("TemplateName") %></a>
                </ItemTemplate>
            </asp:DataList>
        </td>
    </tr>
    <tr>
        <td colspan="2"><b>Optional Info:</b><br />
            Enter links to WSC specific instructions for processing this record-type. Remember to use a persistent URL for storage of these documents.
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Analyzing:<br />
            <telerik:RadTextBox ID="rtbAnalyzeInstructions" runat="server" Height="50px" Width="700px" Skin="Bootstrap" TextMode="MultiLine"  />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            Approving:<br />
            <telerik:RadTextBox ID="rtbApproveInstructions" runat="server" Height="50px" Width="700px" Skin="Bootstrap" TextMode="MultiLine" />
            <hr />
        </td>
    </tr>
    <tr>
        <td><asp:Label ID="lblError" runat="server" /></td>
        <td align="right">
            <telerik:RadButton ID="rbUpdate" Text="Update" runat="server" CommandName="Update" Visible='<%# !(DataItem is Telerik.Web.UI.GridInsertionObject) %>' Skin="Bootstrap" />
            <telerik:RadButton ID="rbInsert" Text="Insert" runat="server" CommandName="PerformInsert" Visible='<%# (DataItem is Telerik.Web.UI.GridInsertionObject) %>' Skin="Bootstrap" />
            &nbsp;
            <telerik:RadButton ID="rbCancel" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" Skin="Bootstrap" />
        </td>
    </tr>
</table>