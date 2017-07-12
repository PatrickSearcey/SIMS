<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecordTypeConfig.ascx.cs" Inherits="RMS.Control.RecordTypeConfig" %>
<table id="Table2" cellspacing="2" cellpadding="1" width="100%" rules="none" border="0">
    <tr class="EditFormHeader">
        <td colspan="2"><b><asp:Label ID="lblHeading" runat="server" /></b></td>
    </tr>
    <tr>
        <td colspan="2"><b>Required Info:</b></td>
    </tr>
    <tr>
        <td colspan="2">
            <table id="Table3" cellspacing="1" cellpadding="1" width="600" border="0">
                <tr>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Record-type code:</td>
                    <td nowrap><asp:TextBox id="tbCode" runat="server" Text='<%# DataBinder.Eval( Container, "DataItem.type_cd" ) %>' />
                    <asp:RequiredFieldValidator ID="rfvCode" runat="server" ControlToValidate="tbCode" Text="You must enter a code" />
                    </td>
                </tr>
                <tr>
                    <td nowrap>Record-type description:</td>
                    <td nowrap><asp:TextBox id="tbDescription" runat="server" Text='<%# DataBinder.Eval( Container, "DataItem.type_ds") %>' Columns="50" tabIndex="1" />
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" 
                        Text="You must enter a description" ControlToValidate="tbDescription"/>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblContorNoncont" runat="server" /></td>
                    <td><asp:Label ID="lblCONStatus" runat="server" />
                        <asp:RadioButtonList ID="rblContorNoncont" runat="server" TabIndex="2">
                            <asp:ListItem Value="cont">Time-series</asp:ListItem>
                            <asp:ListItem Value="noncont">Non-time-series</asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="rfvContorNoncont" runat="server" 
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
        <td colspan="2">
            
        </td>
    </tr>
    <tr>
        <td><asp:Label ID="lblError" runat="server" /></td>
        <td align="right">
            <asp:button id="btnUpdate" text="Update" runat="server" CommandName="Update"></asp:button>
            <asp:button id="btnInsert" text="Insert" runat="server" CommandName="PerformInsert"></asp:button>
            &nbsp;
            <asp:button id="btnCancel" text="Cancel" runat="server" causesvalidation="False" commandname="Cancel"></asp:button>
        </td>
    </tr>
</table>