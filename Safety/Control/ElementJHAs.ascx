<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ElementJHAs.ascx.cs" Inherits="Safety.Control.ElementJHAs" %>

<style type="text/css">
    .Custom .rgCaption
    {
        background-color:#f3efe9;
        font-size: 10pt;
        font-weight: bold;
        color: #26364e;
        padding:3px;
    }
    .Custom .rgMasterTable .rgRow td
    {
        background-color:#099b7a;
        font-size: 12pt;
        font-weight: bold;
        color: white;
        padding: 8px;
    }
    .Custom .rgMasterTable .rgAltRow td
    {
        background-color:#099b7a;
        font-size: 12pt;
        font-weight: bold;
        color: white;
        padding: 8px;
    }
    .Custom .rgDetailTable .rgRow td
    {
        background-color: white;
        color: Black;
        font-size: 10pt;
        font-weight: normal;
        padding: 2px;
    }
    .Custom .rgDetailTable .rgAltRow td
    {
        background-color: white;
        color: Black;
        font-size: 10pt;
        font-weight: normal;
        padding: 2px;
    }
    .Custom .gridImage
    {
        float:right;
    }
    .Custom img 
    { 
        border:none;
    }
    .alert {
        color: #cb2121;
        font-weight:bold;
        font-size: 9pt;
    }
</style>

<script type="text/javascript">
    function OnClientLoad(editor, args) {
        editor.setSize("900px", "400px");
    }
</script>

<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="rgElementHazards">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rgElementHazards" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="lblError2" />
                <telerik:AjaxUpdatedControl ControlID="lblSuccess2" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="lbToggleElementHazardEditMode">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lbToggleElementHazardEditMode" />
                <telerik:AjaxUpdatedControl ControlID="hfToggleElementHazardEditMode" />
                <telerik:AjaxUpdatedControl ControlID="rgElementHazards" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="lblError2" />
                <telerik:AjaxUpdatedControl ControlID="lblSuccess2" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="lbToggleElementEditMode">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lbToggleElementEditMode" />
                <telerik:AjaxUpdatedControl ControlID="hfToggleElementEditMode" />
                <telerik:AjaxUpdatedControl ControlID="pnlStaticElemInfo" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="pnlEditElemInfo" LoadingPanelID="ralp" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnSubmitElemInfo">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblSuccess1" />
                <telerik:AjaxUpdatedControl ControlID="lblError1" />
                <telerik:AjaxUpdatedControl ControlID="ltlElemRevisedInfo" />
                <telerik:AjaxUpdatedControl ControlID="lbToggleElementEditMode" />
                <telerik:AjaxUpdatedControl ControlID="hfToggleElementEditMode" />
                <telerik:AjaxUpdatedControl ControlID="ltlElemInfo" />
                <telerik:AjaxUpdatedControl ControlID="pnlStaticElemInfo" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="pnlEditElemInfo" LoadingPanelID="ralp" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnCloseElemInfoEditing">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="pnlStaticElemInfo" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="pnlEditElemInfo" LoadingPanelID="ralp" />
                <telerik:AjaxUpdatedControl ControlID="lbToggleElementEditMode" />
                <telerik:AjaxUpdatedControl ControlID="hfToggleElementEditMode" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<asp:Panel ID="pnlElement" runat="server" CssClass="roundedPanel">
    <div style="width:100%;height:40px;">
        <div style="float:left;font-size:8pt;">
            <asp:Literal ID="ltlElemRevisedInfo" runat="server" />
            (<asp:Hyperlink ID="hlRevisionHistory" runat="server" Target="_blank">revision history</asp:Hyperlink>)
        </div>
        <div style="text-align:right;">
            <asp:LinkButton ID="lbToggleElementEditMode" runat="server" OnClick="lbToggleElementEditMode_Click" />
            <asp:HiddenField ID="hfToggleElementEditMode" runat="server" />
        </div>
    </div>
    <h4 class="sectionHeadings"><asp:Literal ID="ltlElemName" runat="server" /></h4>
    <asp:Panel ID="pnlStaticElemInfo" runat="server">
        <p>
            <asp:Literal ID="ltlElemInfo" runat="server" />
        </p>
    </asp:Panel>
    <asp:Panel ID="pnlEditElemInfo" runat="server" Visible="false">
        <br />
        <telerik:RadEditor ID="reElemInfo" runat="server" Skin="Bootstrap" OnClientLoad="OnClientLoad" ExternalDialogsPath="~/EditorDialogs/">
            <Tools>
                <telerik:EditorToolGroup>
                    <telerik:EditorTool Name="InsertLink" Text="Insert Link Dialog" />
                    <telerik:EditorTool Name="Bold" Visible="true" /> 
                    <telerik:EditorTool Name="Indent" Text="Indent" Visible="true" />
                    <telerik:EditorTool Name="Outdent" Text="Outdent" Visible="true" />
                    <telerik:EditorTool Name="InsertImage" Text="Insert Image" Visible="true" />
                    <telerik:EditorTool Name="InsertTable" Text="Insert Table" Visible="true" />
                </telerik:EditorToolGroup>
            </Tools>
        </telerik:RadEditor>
        <br />
        <asp:Button id="btnSubmitElemInfo" runat="server" Text="save changes and leave edit mode" OnCommand="btnSubmitElemInfo_Command" CommandArgument="editelement" UseSubmitBehavior="false" />
        <asp:Button ID="btnCloseElemInfoEditing" runat="server" Text="cancel and leave edit mode without saving changes" OnCommand="btnSubmitElemInfo_Command" CommandArgument="closeediting" />
        <p><asp:Label ID="lblError1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
        <asp:Label ID="lblSuccess1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="MediumOrchid"></asp:Label></p>
    </asp:Panel>
    <h5 class="sectionHeadings">Site Specific Job Hazards for <asp:Literal ID="ltlElemName2" runat="server" /></h5>
    <div style="float:left;">
        <asp:Label ID="lblError2" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
        <asp:Label ID="lblSuccess2" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="MediumOrchid"></asp:Label>
    </div>
    <div style="text-align:right;">
        <asp:LinkButton ID="lbToggleElementHazardEditMode" runat="server" OnClick="lbToggleElementHazardEditMode_Click" />
        <asp:HiddenField ID="hfToggleElementHazardEditMode" runat="server" />
    </div>
    <telerik:RadGrid ID="rgElementHazards" runat="server"  
        AllowAutomaticDeletes="true" 
        OnNeedDataSource="rgElementHazards_NeedDataSource" 
        OnDetailTableDataBind="rgElementHazards_DetailTableDataBind"
        OnItemDataBound="rgElementHazards_ItemDataBound"
        OnInsertCommand="rgElementHazards_InsertCommand"
        OnUpdateCommand="rgElementHazards_UpdateCommand"
        OnDeleteCommand="rgElementHazards_DeleteCommand" 
        OnPreRender="rgElementHazards_PreRender"
        Width="100%" Skin="Bootstrap" CssClass="Custom" AllowSorting="false">
        <MasterTableView AutoGenerateColumns="false" DataKeyNames="site_jha_id" ShowHeader="false" HierarchyDefaultExpanded="true">
            <Columns>
                <telerik:GridBoundColumn DataField="site_jha_id" UniqueName="site_jha_id" Display="false" />
                <telerik:GridBoundColumn DataField="jha_description" UniqueName="jha_description" />
                <telerik:GridTemplateColumn UniqueName="TemplateLinks" HeaderStyle-Width="185">
                    <ItemTemplate>
                        <asp:HyperLink ID="hlFullReport" runat="server" ImageUrl="~/images/reports.png" CssClass="gridImage" Target="_blank"  />
                        <telerik:RadToolTip runat="server" ID="rtt0" RelativeTo="Element" Width="150px" AutoCloseDelay="10000" 
                            Height="80px" TargetControlID="hlFullReport" IsClientID="false" Animation="Fade" Position="TopRight" Skin="Bootstrap">
                            View the standardized Job Hazard information for this job.
                        </telerik:RadToolTip>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn ConfirmText="Delete this measurement type?" ButtonType="ImageButton" CommandName="Delete" Text="Delete" ImageUrl="~/images/customdeletebutton.png"
                    UniqueName="DeleteColumn">
                    <HeaderStyle Width="20px" />
                    <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                </telerik:GridButtonColumn>
            </Columns>
            <EditFormSettings EditFormType="Template">
                <FormTemplate>
                    <div style="padding:5px;background-color: #d1ede5;">
                        <h5>Add a new measurement type for this element</h5><br />
                        <telerik:RadDropDownList ID="rddlMeasType" runat="server" DataTextField="jha_description" DataValueField="elem_jha_id" Width="400px" Skin="Bootstrap" /><br /><br />
                        <asp:Button ID="btnInsert1" Text="Insert" runat="server" CommandName="PerformInsert" />
                        <asp:Button ID="btnCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                    </div>
                </FormTemplate>
            </EditFormSettings>
            <CommandItemSettings AddNewRecordText="Add a new job activity for the measurement element" ShowRefreshButton="false" />

            <DetailTables>
                <telerik:GridTableView AutoGenerateColumns="false" AllowSorting="false" width="100%" ShowHeader="false" Name="Hazards" CommandItemDisplay="Bottom" DataKeyNames="site_specificcond_id">
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="site_jha_id" MasterKeyField="site_jha_id" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn1">
                            <HeaderStyle Width="20px" />
                            <ItemStyle  CssClass="MyImageButton" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn DataField="remarks" UniqueName="remarks" />
                        <telerik:GridBoundColumn DataField="priority" UniqueName="priority" Display="false" />
                        <telerik:GridButtonColumn ConfirmText="Delete this hazard?" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                            UniqueName="HazardDeleteColumn">
                            <HeaderStyle Width="20px" />
                            <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                         </telerik:GridButtonColumn>
                    </Columns>
                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <div style="padding:5px;background-color: #d1ede5;">
                                <h5><asp:Literal ID="ltlHazardsEditFormTitle" runat="server" /></h5><br />
                                <asp:TextBox ID="tbHazard" runat="server" Width="400px" Height="100px" TextMode="MultiLine" /><br />
                                <label>Is this a high priority (will be moved to the top of the hazard list, and font bolded)?</label> <asp:CheckBox ID="cbPriority" runat="server" /> <br /><br />
                                <asp:Button ID="btnInsert2" Text="Insert" runat="server" CommandName="PerformInsert" />
                                <asp:Button ID="btnUpdate2" Text="Update" runat="server" CommandName="Update" />
                                <asp:Button ID="btnCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                            </div>
                        </FormTemplate>
                    </EditFormSettings>
                    <CommandItemSettings AddNewRecordText="Add New Hazard" ShowRefreshButton="false" />
                    <NoRecordsTemplate>No hazards have been entered.</NoRecordsTemplate>
                </telerik:GridTableView>

                <telerik:GridTableView AutoGenerateColumns="false" AllowSorting="false" Width="100%" Caption="Job Operational Limits" Name="JobLimits" CommandItemDisplay="Bottom" DataKeyNames="site_reflevel_id">
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="site_jha_id" MasterKeyField="site_jha_id" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn2">
                            <HeaderStyle Width="20px" />
                            <ItemStyle  CssClass="MyImageButton" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn DataField="reflevel_va" UniqueName="reflevel_va" HeaderText="Limit Value" HeaderStyle-Width="80px" />
                        <telerik:GridBoundColumn DataField="reflevel_units" UniqueName="reflevel_units" HeaderText="Limit Units" HeaderStyle-Width="80px" />
                        <telerik:GridBoundColumn DataField="reflevel_desc" UniqueName="reflevel_desc" HeaderText="Limit Type" HeaderStyle-Width="250px" />
                        <telerik:GridBoundColumn DataField="remarks" UniqueName="reflevelremarks" HeaderText="Remarks" />
                        <telerik:GridButtonColumn ConfirmText="Delete this operational limit?" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                            UniqueName="JobLimitDeleteColumn" HeaderStyle-Width="20px">
                            <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                         </telerik:GridButtonColumn>
                    </Columns>
                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <div style="padding:5px;background-color: #d1ede5;">
                                <h5><asp:Literal ID="ltlJobLimitsEditFormTitle" runat="server" /></h5>
                                <table cellspacing="0" cellpadding="5" width="100%" border="0" rules="none">
                                    <tr>
                                        <td colspan="2"><p class="alert">Due to potential changes in stream profile over time the Maximum Wading Gage Height (MWGH) has been changed 
                                            to a "Recommended MWGH". It is also imperative that the measurement location be specified as a "Remark" in the Job Operational Limit.</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:120px;"><label>Job limit type:</label></td>
                                        <td><telerik:RadDropDownList ID="rddlJobLimitType" runat="server" DataTextField="reflevel_tp_desc" DataValueField="reflevel_id" Skin="Bootstrap" Width="400px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:120px;"><label>Job limit value:</label></td>
                                        <td><telerik:RadNumericTextBox ID="rntbJobLimitValue" runat="server" Width="80px" />
                                            <asp:RequiredFieldValidator ID="rfvJobLimitValue" runat="server" ControlToValidate="rntbJobLimitValue" ErrorMessage="*" ForeColor="Red" />
                                            <label>units:</label> <asp:TextBox ID="tbUnits" runat="server" Width="70px" />
                                            <asp:RequiredFieldValidator ID="rfvUnits" runat="server" ControlToValidate="tbUnits" ErrorMessage="* required" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:120px;"><label>Remarks:</label></td>
                                        <td><asp:TextBox ID="tbRemarks" runat="server" Width="400px" Height="100px" TextMode="MultiLine" />
                                            <asp:RequiredFieldValidator ID="rfvRemarks" runat="server" ControlToValidate="tbRemarks" ErrorMessage="* Remarks are now required" ForeColor="Red" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Button ID="btnInsert3" Text="Insert" runat="server" CommandName="PerformInsert" />
                                <asp:Button ID="btnUpdate3" Text="Update" runat="server" CommandName="Update" />
                                <asp:Button ID="btnCancel3" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                            </div>
                        </FormTemplate>
                    </EditFormSettings>
                    <CommandItemSettings AddNewRecordText="Add New Job Operational Limit" ShowRefreshButton="false" />
                    <NoRecordsTemplate>No job operational limits have been entered.</NoRecordsTemplate>
                </telerik:GridTableView>

            </DetailTables>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Panel>