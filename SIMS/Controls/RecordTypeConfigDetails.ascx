<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecordTypeConfigDetails.ascx.vb" Inherits="SIMS.RecordTypeConfigDetails" %>
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
        <td colspan="2"><b>Optional Info:</b></td>
    </tr>
    <tr>
        <td colspan="2">Enter links to WSC specific instructions for processing this record-type. Remember to use a persistent
        URL for storage of these documents.</td>
    </tr>
    <tr>
        <td colspan="2">
            <table id="Table1" cellspacing="1" cellpadding="1" width="600" border="0">
                <tr>
                    <td>Working:</td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox id="tbWorkInst" Text='<%# DataBinder.Eval( Container, "DataItem.work_html_va") %>' runat="server" TextMode="MultiLine" Rows="5" Columns="100" tabIndex="3" />
                    </td>
                </tr>
                <tr>
                    <td>Checking:</td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox id="tbCheckInst" Text='<%# DataBinder.Eval( Container, "DataItem.check_html_va") %>' runat="server" TextMode="MultiLine" Rows="5" Columns="100" tabIndex="4" />
                    </td>
                </tr>
                <tr>
                    <td>Reviewing:</td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox id="tbReviewInst" Text='<%# DataBinder.Eval( Container, "DataItem.review_html_va") %>' runat="server" TextMode="MultiLine" Rows="5" Columns="100" tabIndex="5" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td><asp:Label ID="lblError" runat="server" /></td>
        <td align="right">
            <asp:button id="btnUpdate" text="Update" runat="server" CommandName="Update" Visible='<%# Not (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>'></asp:button>
            <asp:button id="btnInsert" text="Insert" runat="server" CommandName="PerformInsert" Visible='<%# (TypeOf DataItem Is Telerik.Web.UI.GridInsertionObject) %>'></asp:button>
            &nbsp;
            <asp:button id="btnCancel" text="Cancel" runat="server" causesvalidation="False" commandname="Cancel"></asp:button>
        </td>
    </tr>
</table>