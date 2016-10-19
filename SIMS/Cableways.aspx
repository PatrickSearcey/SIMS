<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSafety.Master" CodeBehind="Cableways.aspx.vb" Inherits="SIMS.Cableways" %>
<%@ MasterType  virtualPath="~/SIMSSafety.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
.basicPanel 
{
    padding-top:10px;
    padding-bottom:10px;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
    <asp:Panel ID="pnlHasAccess" runat="server">
        <telerik:RadScriptBlock ID="rsb1" runat="server">
            <script type="text/javascript">
                //On insert and update buttons click temporarily disables ajax to perform upload actions
                function conditionalPostback(sender, e) {
                    var theRegexp = new RegExp("\.UpdateButton$|\.PerformInsertButton$", "ig");
                    if (e.EventTarget.match(theRegexp)) {
                        var upload = $find(uploadId);

                        //AJAX is disabled only if file is selected for upload
                        if (upload.getFileInputs()[0].value != "") {
                            e.set_enableAjax(false);
                        }
                    }
                }
            </script>
        </telerik:RadScriptBlock>
        <div style="height:50px;width:100%;padding-bottom:10px;">
            <p style="font-size:9pt;">A cableway is defined as any <b>permanent bank-supported</b> aerial conveying system suspended above a waterway for the 
            purpose of making hydrologic measurements. Cableways typically are classified as either: (1) <b>manned cableways</b> where the 
            hydrographer traverses the river in a cable car suspended from the main cable to operate measurement equipment; or (2) 
            <b>bank-operated cableways</b> (BOS) where the hydrographer is positioned on the stream bank to remotely operate cableway-
            suspended measurement equipment.</p>
        </div>
        <div style="height:60px;width:100%;">
            <div style="float:left;">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Green"></asp:Label><br />
                <asp:LinkButton ID="lbToggleRStatus" runat="server" OnCommand="lbToggleRStatus_Command" Font-Bold="true" />
            </div>
            <div style="float:right;">
                <asp:ImageMap ID="imInstructions" runat="server" ImageUrl="images/manualandhelplinks.png" BorderStyle="None">
                    <asp:RectangleHotSpot AlternateText="Manned Cableway Inspection e-Form" HotSpotMode="Navigate" 
                        NavigateUrl="Docs/MannedCablewayInspectionE-form.pdf" Bottom="33" Left="38" Right="210" Top="15" Target="_blank" />
                    <asp:RectangleHotSpot AlternateText="Bank-Operated Cableways Inspection Checklist" HotSpotMode="Navigate" 
                        NavigateUrl="Docs/Bank-OperatedCablewayInspectionE-form.pdf" Bottom="58" Left="38" Right="210" Top="38" Target="_blank" />
                    <asp:RectangleHotSpot AlternateText="USGS Manual" HotSpotMode="Navigate" 
                        NavigateUrl="http://www.usgs.gov/usgs-manual/handbook/hb/445-2-h/ch41.html" Bottom="33" Left="270" 
                        Right="470" Target="_blank" Top="10" />
                    <asp:RectangleHotSpot AlternateText="WMA Memo 13.03" HotSpotMode="Navigate"
                        NavigateUrl="Docs/WMAMemorandum13.03.pdf" Bottom="60" Left="270" Right="460" Target="_blank" Top="38" />
                    <asp:RectangleHotSpot AlternateText="CMI Instructions (.pptx)" HotSpotMode="Navigate" 
                        NavigateUrl="Docs/SIMSCMIInstructions.pptx" Left="520" 
                        Right="800" Target="_blank" Top="5" Bottom="60" />
                </asp:ImageMap>
            </div>
        </div>
        <telerik:RadGrid ID="rgCableways" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
            Skin="Web20" GridLines="None" ShowStatusBar="true" PageSize="50"
            AllowSorting="true" 
            AllowMultiRowSelection="false" 
            AllowFiltering="true"
            AllowPaging="true"
            AllowAutomaticDeletes="true" 
            OnItemDataBound="rgCableways_ItemDataBound"
            OnInsertCommand="rgCableways_InsertCommand"
            OnUpdateCommand="rgCableways_UpdateCommand"
            OnDeleteCommand="rgCableways_DeleteCommand"
            OnPreRender="rgCableways_PreRender">
            <PagerStyle Mode="NumericPages" />
            <MasterTableView DataKeyNames="cableway_id" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="Top" 
                Name="Cableways" AllowFilteringByColumn="true">
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="cableway_visit_id" Width="100%" runat="server" CommandItemDisplay="Top"
                        Name="Visits">
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="cableway_id" MasterKeyField="cableway_id" />
                        </ParentTableRelation>  
                        <CommandItemSettings AddNewRecordText="Add New Visit" ShowRefreshButton="false" />
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn2">
                                <HeaderStyle Width="20px" />
                                <ItemStyle CssClass="MyImageButton" />
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn DataField="visit_dt" HeaderText="Visit Date" DataFormatString="{0:MM/dd/yyyy}" 
                                UniqueName="visit_dt" SortExpression="visit_dt" AllowFiltering="false" HeaderStyle-Width="100" />
                            <telerik:GridBoundColumn DataField="type_cd_desc" HeaderText="Visit Type" UniqueName="type_cd_desc" HeaderStyle-Width="180" 
                                SortExpression="visit_type_cd" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="visit_type_cd" UniqueName="visit_type_cd" Display="false" ReadOnly="true" />
                            <telerik:GridBoundColumn DataField="action_cd_desc" HeaderText="Visit Action" UniqueName="action_cd_desc" HeaderStyle-Width="220"
                                SortExpression="visit_action_cd" AllowFiltering="false" />
                            <telerik:GridBoundColumn DataField="visit_action_cd" Display="false" ReadOnly="true" UniqueName="visit_action_cd" />
                            <telerik:GridTemplateColumn HeaderText="Documentation" HeaderStyle-Width="300" DataField="visit_file_nm">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlDoc" runat="server" Text='<%# Bind("visit_file_nm") %>' Target="_blank" />&nbsp;
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="remarks" HeaderText="Remarks" SortExpression="remarks" UniqueName="remarks" AllowFiltering="false" />
                            <telerik:GridButtonColumn ConfirmText="Delete this visit?" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                UniqueName="VisitDeleteColumn">
                                <HeaderStyle Width="20px" />
                                <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                            </telerik:GridButtonColumn>
                        </Columns>

                        <EditFormSettings EditFormType="Template">
                            <FormTemplate>
                                <div style="padding:5px;background-color:#f4cebf;">
                                    <table id="tableForm1" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: #f4cebf;">
                                        <tr>
                                            <td colspan="2">
                                                <h4><asp:Literal ID="ltlDetailsEditFormTitle" runat="server" /></h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width:120px;">
                                                <label>Visit Date:</label>
                                            </td>
                                            <td>
                                                <telerik:RadDatePicker ID="rdpVisit" runat="server" MinDate="1/1/1900" />
                                                <asp:Image ID="imgVisitDateHelp" runat="server" ImageURL="~/Images/tooltip.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Visit Type:</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlVisitType" runat="server" DataTextField="type_cd_desc"
                                                    DataValueField="visit_type_cd" Width="400px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Cableway Assessment:</label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlVisitAction" runat="server" DataTextField="action_cd_desc"
                                                    DataValueField="visit_action_cd" Width="400px" />
                                                <asp:Image ID="imgVisitActionHelp" runat="server" ImageURL="~/Images/tooltip.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label>Remarks:</label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="tbRemarks" runat="server" Width="400px" Height="100px" TextMode="MultiLine" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <label><asp:Label ID="lblUploadDoc" runat="server" Text="Upload Document: " /></label>
                                            </td>
                                            <td>
                                                <div style="float:left;">
                                                <telerik:RadUpload ID="fuFile" runat="server" Skin="Web20" InitialFileInputsCount="1" Width="250px"
                                                    MaxFileInputsCount="1" ControlObjectsVisibility="None" MaxFileSize="524288000" />
                                                </div>
                                                <asp:Image ID="imgUploadDocHelp" runat="server" ImageURL="~/Images/tooltip.png" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" colspan="2">
                                                <asp:Button ID="btnInsert1" Text="Insert" runat="server" CommandName="PerformInsert" />
                                                <asp:Button ID="btnUpdate1" Text="Update" runat="server" CommandName="Update" />
                                                <asp:Button ID="btnCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <telerik:RadToolTip runat="server" ID="rtt99" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                    Height="50px" TargetControlID="imgVisitDateHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    Enter the date of the visit. If you are adding non-visit information, enter today's date.
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt0" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                    Height="50px" TargetControlID="imgUploadDocHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    One supporting document may be uploaded. Please upload any hand written inspection forms. To include images or other supporting documents, please merge them into one file.   
                                </telerik:RadToolTip>
                                <telerik:RadToolTip runat="server" ID="rtt1" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                                    Height="50px" TargetControlID="imgVisitActionHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                                    What was the outcome of this visit? What you choose here might impact your Cableway Status, and you may need to edit the cableway above as well.
                                </telerik:RadToolTip>
                            </FormTemplate>
                        </EditFormSettings>
                        <NoRecordsTemplate>
                            <div style="background-color:#d7adad;padding:5px;font-weight:bold;">No Site Visits have been recorded.</div>
                        </NoRecordsTemplate>
                        <EditItemStyle BackColor="#f4cebf" />
                        <ItemStyle BackColor="#f4eebf" />
                        <AlternatingItemStyle BackColor="#f4eebf" />
                    </telerik:GridTableView>
                </DetailTables>
                <CommandItemSettings AddNewRecordText="Add New Cableway" ShowRefreshButton="false" />
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn1">
                        <HeaderStyle Width="20px" />
                        <ItemStyle CssClass="MyImageButton" />
                    </telerik:GridEditCommandColumn>
                    <telerik:GridBoundColumn DataField="cableway_id" UniqueName="cableway_id" Display="false" ReadOnly="true"  />
                    <telerik:GridBoundColumn DataField="site_id" UniqueName="site_id" Display="false" ReadOnly="true" />
                    <telerik:GridBoundColumn DataField="cableway_nm" UniqueName="cableway_nm" Display="false" ReadOnly="true" />
                    <telerik:GridBoundColumn DataField="site_no_nm" HeaderText="Site" UniqueName="site_no_nm" SortExpression="site_no" FilterControlWidth="150px" />
                    <telerik:GridBoundColumn DataField="cableway_status_cd" UniqueName="cableway_status_cd" Display="false" ReadOnly="true" />
                    <telerik:GridBoundColumn DataField="status_cd_desc" HeaderText="Cableway Status" UniqueName="status_cd_desc" SortExpression="cableway_status_cd" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="cableway_type_cd" UniqueName="cableway_type_cd" Display="false" ReadOnly="true" />
                    <telerik:GridBoundColumn DataField="type_cd_desc" HeaderText="Cableway Type"  UniqueName="type_cd_desc" SortExpression="cableway_type_cd" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="cableway_inspection_freq" HeaderText="Insp Freq" UniqueName="cableway_inspection_freq" SortExpression="cableway_inspection_freq" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="aerial_marker_req" HeaderText="Aerial Marker Required" UniqueName="aerial_marker_req" SortExpression="aerial_marker_req" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="aerial_marker_inst" HeaderText="Aerial Marker Installed" UniqueName="aerial_marker_inst" SortExpression="aerial_marker_inst" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="office_cd" HeaderText="Office" UniqueName="office_cd" ReadOnly="true" FilterControlWidth="30px" />
                </Columns>

                <EditFormSettings EditFormType="Template">
                    <FormTemplate>
                        <div style="padding:5px;background-color:#cfe3db;">
                            <table id="tableForm1" cellspacing="5" cellpadding="5" width="600" border="0" rules="none" style="border-collapse: collapse; background: #cfe3db;">
                                <tr>
                                    <td colspan="2">
                                        <h4><asp:Literal ID="ltlEditFormTitle" runat="server" /></h4>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width:120px;">
                                        <label>Site:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSites" runat="server" Width="400px" DataTextField="site_no_nm" DataValueField="site_id" />
                                        <asp:Image ID="imgSitesHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Nickname:</label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="tbNickname" runat="server" Width="200px" MaxLength="30" />
                                        <asp:Image ID="imgNicknameHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Cableway Status:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCablewayStatus" runat="server" Width="400px" DataTextField="status_cd_desc" DataValueField="cableway_status_cd" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Cableway Type:</label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCablewayType" runat="server" Width="400px" DataTextField="type_cd_desc" DataValueField="cableway_type_cd" />
                                        <asp:Image ID="imgCablewayTypeHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Inspection Frequency:</label>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="rntbFreq" runat="server" Width="50px" NumberFormat-DecimalDigits="0" MaxLength="3" />
                                        <asp:Image ID="imgFreqHelp" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Are Aerial Markers:</label><br />
                                        <p style="padding-left:40px;padding-bottom:3px;"><label>Required?</label></p>
                                        <p style="padding-left:40px;"><label>Installed?</label></p>
                                    </td>
                                    <td>
                                        <br />
                                        <asp:RadioButtonList ID="rblAerialMarkerReq" runat="server" RepeatDirection="Horizontal" Style="float:left;">
                                            <asp:ListItem Text="Yes" Value="Y" />
                                            <asp:ListItem Text="No" Value="N" />
                                            <asp:ListItem Text="Unknown" Value="U" />
                                        </asp:RadioButtonList>
                                        &nbsp;&nbsp;<asp:Image ID="imgAerialMarkerReq" runat="server" ImageUrl="~/Images/tooltip.png" /><br /><br />
                                        <asp:RadioButtonList ID="rblAerialMarkerInst" runat="server" RepeatDirection="Horizontal" Style="float:left;">
                                            <asp:ListItem Text="Yes" Value="Y" />
                                            <asp:ListItem Text="No" Value="N" />
                                            <asp:ListItem Text="Unknown" Value="U" />
                                        </asp:RadioButtonList>
                                        &nbsp;&nbsp;<asp:Image ID="imgAerialMarkerInst" runat="server" ImageUrl="~/Images/tooltip.png" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="2">
                                        <asp:Button ID="btnInsert2" Text="Insert" runat="server" CommandName="PerformInsert" />
                                        <asp:Button ID="btnUpdate2" Text="Update" runat="server" CommandName="Update" />
                                        <asp:Button ID="btnCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <telerik:RadToolTip runat="server" ID="rtt3" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="50px" TargetControlID="imgSitesHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            Only sites that already exist in SIMS are listed.  If you do not see your site, you must first add it to SIMS.  
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rrt5" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="50px" TargetControlID="imgNicknameHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            Enter a short descriptive phrase or word for this cableway. This is useful for setting your cableways apart if you have multiples at a site.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt6" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="50px" TargetControlID="imgCablewayTypeHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            <b>Bank Operated Cableway</b> A manually operated "loop" cableway<br />
                            <b>Manned Cableway</b> Traditional cableway with cable car and operator<br />
                            <b>Type Unknown</b> You just don't know
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt7" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="50px" TargetControlID="imgFreqHelp" IsClientID="false" Animation="Fade" Position="TopRight">
                            Enter in years the frequency of inspection visits. This is a requirement to operate the cableway.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt8" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            height="50px" TargetControlID="imgAerialMarkerReq" IsClientID="false" Animation="Fade" Position="TopRight">
                            Select yes if an aerial marker is required for this cableway, or no if it is not.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt9" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            height="50px" TargetControlID="imgAerialMarkerInst" IsClientID="false" Animation="Fade" Position="TopRight">
                            Select yes if an aerial marker exists for this cableway, or no if it does not.
                        </telerik:RadToolTip>
                    </FormTemplate>
                </EditFormSettings>
                <EditItemStyle BackColor="#cfe3db" />
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
