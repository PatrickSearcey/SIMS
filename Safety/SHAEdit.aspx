<%@ Page Title="" Language="C#" MasterPageFile="~/SafetySingleMenu.Master" AutoEventWireup="true" CodeBehind="SHAEdit.aspx.cs" Inherits="Safety.SHAEdit" %>
<%@ Register TagPrefix="uc" TagName="ElementJHAs" Src="~/Control/ElementJHAs.ascx" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/reports.css" rel="stylesheet" />
    <script type="text/javascript">
        function ShowDeleteHospitalForm(id, rowIndex, sha_site_id) {
            var grid = $find("<%= rgHospitals.ClientID %>");

            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);

            window.radopen("Modal/DeleteEmergencyInfo.aspx?hospital_id=" + id + "&sha_site_id=" + sha_site_id, "DeleteDialog");
            return false;
        }
        function ShowDeleteContactForm(id, rowIndex, sha_site_id) {
            var grid = $find("<%= rgContacts.ClientID %>");

            var rowControl = grid.get_masterTableView().get_dataItems()[rowIndex].get_element();
            grid.get_masterTableView().selectItem(rowControl, true);

            window.radopen("Modal/DeleteEmergencyInfo.aspx?contact_id=" + id + "&sha_site_id=" + sha_site_id, "DeleteDialog");
            return false;
        }
        function refreshGrid(arg) {
            if (!arg) {
                $find("<%= ram.ClientID %>").ajaxRequest("Rebind");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ibWarningsOpen">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlWarningList" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="blWarnings" />
                    <telerik:AjaxUpdatedControl ControlID="ibWarningsOpen" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ibWarningsClose" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ibWarningsClose">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlWarningList" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="blWarnings" />
                    <telerik:AjaxUpdatedControl ControlID="ibWarningsOpen" />
                    <telerik:AjaxUpdatedControl ControlID="ibWarningsClose" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbToggleHazardEditMode">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbToggleHazardEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="hfToggleHazardEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="lvServicingSiteSpecificCond" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbToggleEquipEditMode">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbToggleEquipEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="hfToggleEquipEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="lvServicingSiteRecEquip" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lvServicingSiteRecEquip">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbToggleEquipEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="hfToggleEquipEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbAddSiteSpecificInfo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="hfAddSiteSpecificInfo" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAddSiteSpecificInfo" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbAddSiteSpecificInfo" />
                    <telerik:AjaxUpdatedControl ControlID="imgArrow1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbCloseAddSiteSpecificInfo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="hfAddSiteSpecificInfo" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAddSiteSpecificInfo" />
                    <telerik:AjaxUpdatedControl ControlID="lbAddSiteSpecificInfo" />
                    <telerik:AjaxUpdatedControl ControlID="imgArrow1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAddHazards">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lvServicingSiteSpecificCond" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbToggleHazardEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="rlbHazards" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAddEquip">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lvServicingSiteRecEquip" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbToggleEquipEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="rlbEquip" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lvServicingSiteSpecificCond">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lvServicingSiteSpecificCond" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="lbToggleHazardEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="hfToggleHazardEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnNewMeasType">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="phElements" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNewMeasType" />
                    <telerik:AjaxUpdatedControl ControlID="ddlNewMeasType" />
                    <telerik:AjaxUpdatedControl ControlID="btnNewMeasType" />
                    <telerik:AjaxUpdatedControl ControlID="lblError1" />
                    <telerik:AjaxUpdatedControl ControlID="lblSuccess1" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
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
                    <telerik:AjaxUpdatedControl ControlID="lblSuccess2" />
                    <telerik:AjaxUpdatedControl ControlID="lblError2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlElemRevisedInfo" />
                    <telerik:AjaxUpdatedControl ControlID="lbToggleElementEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="hfToggleElementEditMode" />
                    <telerik:AjaxUpdatedControl ControlID="ltlElemInfo" />
                    <telerik:AjaxUpdatedControl ControlID="pnlStaticElemInfo" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlEditElemInfo" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
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
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgHospitals" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="rgContacts" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgHospitals">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgHospitals" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice1" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgContacts">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgContacts" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice2" />
                    <telerik:AjaxUpdatedControl ControlID="ltlNotice1" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbReview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlReview" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlReviewSubmit" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReviewed">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnCancelReview">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbApprove">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlApprovePreSubmit" />
                    <telerik:AjaxUpdatedControl ControlID="pnlApproveSubmit" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnApproved">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnCancelApprove">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lb911Service">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbl911Service" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbCellService">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblCellService" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAdmin" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap">
    </telerik:RadAjaxLoadingPanel>

    <uc:PageHeading id="ph1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <asp:Panel ID="pnlHasAccess" runat="server" CssClass="pnlHasAccess">

        <!-- PRINT VERSION LINK -->
        <div style="float:right;padding-top:0;margin-top:0;">
            <asp:HyperLink ID="hlPrintSHA" runat="server" ImageUrl="images/printSHAlink.png" Target="_blank" />
        </div>

        <!-- SAFETY WARNINGS -->
        <div class="basicPanel" style="float:left;margin-top:0;">
            <asp:Panel ID="pnlWarnings" runat="server">
                <asp:ImageButton ID="ibWarningsOpen" runat="server" ImageUrl="images/SHAWarningsOpen.png" OnCommand="ibWarnings_Command" CommandArgument="open" Visible="false" />
                <asp:ImageButton ID="ibWarningsClose" runat="server" ImageUrl="images/SHAWarningsClose.png" OnCommand="ibWarnings_Command" CommandArgument="close" />
                <asp:Panel ID="pnlWarningList" runat="server">
                    <asp:BulletedList ID="blWarnings" runat="server" ForeColor="Red">
                    </asp:BulletedList>
                </asp:Panel>
            </asp:Panel>
        </div>

        <!-- SAFELY SERVICING THIS SITE -->
        <div class="basicPanel">
            <h3 class="sectionHeadings">Safely Servicing This Site</h3>
            <p>Enter notes specific to visiting or servicing the site. Do not include information specific to each measurement preformed at the site.</p>
            <asp:Panel ID="pnlServicingSite" runat="server" CssClass="roundedPanel">
                <h4 class="sectionHeadings">General Hazards at this Site</h4>
                <div style="float:right;font-weight:bold;font-size:8pt;">
                    <asp:LinkButton ID="lbToggleHazardEditMode" runat="server" OnClick="lbToggleHazardEditMode_Click" />
                    <asp:HiddenField ID="hfToggleHazardEditMode" runat="server" />
                </div>
                <div style="padding:10px 0 0 20px;">
                    <asp:ListView runat="server" ID="lvServicingSiteSpecificCond" OnItemDataBound="lvServicingSiteSpecificCond_ItemDataBound">
                        <LayoutTemplate>
                            <ul>
                                <li runat="server" id="itemPlaceholder"></li>
                            </ul>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <li>
                                <asp:Label ID="lblServicingSiteSpecificCond" runat="server" Text='<%#Eval("servicing_va") %>' />
                                <asp:TextBox id="tbServicingSiteSpecificCond" runat="server" Text='<%#Eval("servicing_va") %>' Width="400px" Height="50px" TextMode="MultiLine" />
                                <asp:ImageButton ID="ibEditHazard" runat="server" OnCommand="ibHazard_Command" CommandArgument='<%#Eval("site_servicing_id") %>' CommandName="EditHazard" ImageUrl="images/edit.png" />
                                <asp:ImageButton ID="ibDeleteHazard" runat="server" OnCommand="ibHazard_Command" CommandArgument='<%#Eval("site_servicing_id") %>'  CommandName="DeleteHazard" ImageUrl="images/delete.png" />
                                <asp:ImageButton ID="ibChangePriority" runat="server" OnCommand="ibHazard_Command" CommandArgument='<%#Eval("site_servicing_id") + "_" + Eval("priority") %>' ComamndName="Priority" ImageUrl="images/priority.png" />
                        </li>
                        <telerik:RadToolTip runat="server" ID="rtt0" RelativeTo="Element" Width="150px" AutoCloseDelay="10000"
                            Height="30px" TargetControlID="ibDeleteHazard" IsClientID="false" Animation="Fade" Position="TopRight" Skin="Bootstrap">
                            Delete this remark.
                        </telerik:RadToolTip>
                        <telerik:RadToolTip runat="server" ID="rtt1" RelativeTo="Element" Width="300px" AutoCloseDelay="10000"
                            Height="50px" TargetControlID="ibChangePriority" IsClientID="false" Animation="Fade" Position="TopRight" Skin="Bootstrap">
                            Change the priority of the remark (making a remark a priority bolds the font and moves it above the non-bold items).
                        </telerik:RadToolTip>
                        <telerik:RadToolTip ID="rtt2" runat="server" relativeto="Element" Width="300px" AutoCloseDelay="10000" 
                            Height="30px" TargetControlID="ibEditHazard" IsClientID="false" Animation="Fade" Position="TopRight" Skin="Bootstrap">
                            Edit this remark.
                        </telerik:RadToolTip>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <li>No servicing site specific hazards have been added.</li>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlRecEquipment" runat="server" CssClass="roundedPanel">
            <h4 class="sectionHeadings">Required PPE & Recommended Equipment Regardless of Activity</h4>
            <div style="float:right;font-weight:bold;font-size:8pt;">
                <asp:LinkButton ID="lbToggleEquipEditMode" runat="server" OnClick="lbToggleEquipEditMode_Click" />
                <asp:HiddenField ID="hfToggleEquipEditMode" runat="server" />
            </div>
            <div style="padding:10px 0 0 20px;">
                <asp:ListView runat="server" ID="lvServicingSiteRecEquip" OnItemDataBound="lvServicingSiteRecEquip_ItemDataBound">
                    <LayoutTemplate>
                        <ul>
                            <li runat="server" id="itemPlaceholder"></li>
                        </ul>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:Label ID="lblServicingSiteRecEquip" runat="server" Text='<%#Eval("recom_equip") %>' />
                            <asp:ImageButton ID="ibDeleteEquip" runat="server" OnCommand="ibEquip_Command" CommandArgument='<%#Eval("site_equip_id") %>' CommandName="DeleteEquip" ImageUrl="images/delete.png" />
                        </li>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <li>No equipment has been added.</li>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </asp:Panel>
        <p><asp:Image ID="imgArrow1" runat="server" ImageUrl="images/arrowbullet.png" AlternateText="arrow" CssClass="floatLeft" /> 
        <asp:LinkButton ID="lbAddSiteSpecificInfo" runat="server" OnCommand="lbAddSiteSpecificInfo_Command" CommandArgument="open" Font-Size="Medium">&nbsp;Need to add items 
        to the above lists? Click here!</asp:LinkButton></p>
        <asp:HiddenField ID="hfAddSiteSpecificInfo" runat="server" />
        <asp:Panel ID="pnlAddSiteSpecificInfo" runat="server" CssClass="addServicingSiteInfo">
            <h4 class="sectionHeadings">Add Servicing Site Specific Info</h4>
            <asp:Panel ID="pnlAddHazard" runat="server" CssClass="leftpanel">
                <p style="font-weight:bold;">Add Hazard Info</p>
                <telerik:RadListBox ID="rlbHazards" runat="server" CheckBoxes="false" Width="400px" Height="150px" SelectionMode="Multiple">
                    <Items>
                        <telerik:RadListBoxItem Text="aircraft required" />
                        <telerik:RadListBoxItem Text="compressed gas cylinders tansport and storage (secured)" />
                        <telerik:RadListBoxItem Text="dangerous animals: cattle, wild hogs, bears, mountain lions, etc." />
                        <telerik:RadListBoxItem Text="fall protection required" />
                        <telerik:RadListBoxItem Text="fencing (electric fences or barb wire)" />
                        <telerik:RadListBoxItem Text="gates" />
                        <telerik:RadListBoxItem Text="high speed traffic" />
                        <telerik:RadListBoxItem Text="hostile encounters (rural or urban areas)" />
                        <telerik:RadListBoxItem Text="infectious diseases and rodents (Hantavirus, Lyme)" />
                        <telerik:RadListBoxItem Text="insects - wasps, mosquitoes, ticks and other biting insects" />
                        <telerik:RadListBoxItem Text="poisonous plants (Poison Oak, Ivym etc)" />
                        <telerik:RadListBoxItem Text="potential for rapidly changing stage (dam releases, flood response)" />
                        <telerik:RadListBoxItem Text="quicksand" />
                        <telerik:RadListBoxItem Text="remote areas (satellite phone, cell phone, spot needed, call-in procedure reviewed)" />
                        <telerik:RadListBoxItem Text="road shoulder less than 5 feet" />
                        <telerik:RadListBoxItem Text="slippery slope or concrete channels" />
                        <telerik:RadListBoxItem Text="snakes" />
                        <telerik:RadListBoxItem Text="special equipment or training needed" />
                        <telerik:RadListBoxItem Text="stilling wells with confined space hazards, permit or non-permit status" />
                        <telerik:RadListBoxItem Text="structural safety (walkways, stairs, ladders, floors, and handrails at site)" />
                        <telerik:RadListBoxItem Text="unsanitary conditions (near waste treatment, near homeless camps, etc)" />
                    </Items>
                </telerik:RadListBox>
                <p style="padding-top:2px;padding-bottom:2px;">
                <label>Other:</label> <asp:TextBox ID="tbOtherHazard" runat="server" /></p>
                <telerik:RadButton ID="btnAddHazards" runat="server" Text="Add selected hazards" OnCommand="AddInfo_Command" CommandName="ServicingSite" CommandArgument="AddHazard" AutoPostBack="true" />
            </asp:Panel>
            <asp:Panel ID="pnlAddEquip" runat="server" CssClass="rightpanel">
                <p style="font-weight:bold;">Add Required PPE & Recommended Equipment</p>
                <telerik:RadListBox ID="rlbEquip" runat="server" CheckBoxes="false" Width="250px" Height="150px" SelectionMode="Multiple">
                    <Items>
                        <telerik:RadListBoxItem Text="cell phone" />
                        <telerik:RadListBoxItem Text="first aid kit" />
                        <telerik:RadListBoxItem Text="flags and stands" />
                        <telerik:RadListBoxItem Text="gloves" />
                        <telerik:RadListBoxItem Text="hat" />
                        <telerik:RadListBoxItem Text="HiVis Class 3 Traffic Vest" />
                        <telerik:RadListBoxItem Text="ice cleats" />
                        <telerik:RadListBoxItem Text="insect repellent" />
                        <telerik:RadListBoxItem Text="PFD" />
                        <telerik:RadListBoxItem Text="proper foot wear" />
                        <telerik:RadListBoxItem Text="protective clothing" />
                        <telerik:RadListBoxItem Text="rope" />
                        <telerik:RadListBoxItem Text="satellite phone" />
                        <telerik:RadListBoxItem Text="shovel (digging tools)" />
                        <telerik:RadListBoxItem Text="sun block" />
                        <telerik:RadListBoxItem Text="sunglasses" />
                        <telerik:RadListBoxItem Text="weather gear" />
                        <telerik:RadListBoxItem Text="Survival Pack - with snacks" />
                    </Items>
                </telerik:RadListBox>
                <p style="padding-top:2px;padding-bottom:2px;">
                <label>Other:</label> <asp:TextBox ID="tbOtherEquip" runat="server" /></p>
                <telerik:RadButton ID="btnAddEquip" runat="server" Text="Add selected equipment" OnCommand="AddInfo_Command" CommandName="ServicingSite" CommandArgument="AddEquip" AutoPostBack="true" />
            </asp:Panel>
            <div style="margin: 3px 0 0 0;font-size: 8pt;width:100%;text-align:right;"><asp:LinkButton ID="lbCloseAddSiteSpecificInfo" runat="server" OnCommand="lbAddSiteSpecificInfo_Command" CommandArgument="close">Done adding? 
            Click to close this section.</asp:LinkButton></div>
        </asp:Panel>
    </div>

    <!-- MAKING SAFE MEASUREMENTS -->
    <div class="basicPanel">
        <h3 class="sectionHeadings">Making Safe Measurements</h3>
        <p>Information below is also editable from the element editing interface in SIMS. Elements are also available in the Station Description View. 
        New Measurement Type Elements can be added from the SIMS element editing interface or by choosing from this list at the bottom of this section.</p>
        <div class="subPanel">
            <h4 class="sectionHeadings">Site Specific Job Hazards by Measurement Type</h4>
            <p>Potential safety hazards associated with making measurements. Standardized Job Hazard information for each activity can be viewed by
            clicking on the <img src="images/reports.png" alt="Standard JHA Info" /> icon.  <strong><a href="http://1stop.usgs.gov/safety/topic/jha/index.html" target="_blank">Click here for the 
            1stop.usgs.gov JHA Index.</a></strong></p>
            <asp:PlaceHolder ID="phElements" runat="server" />
            <h4 class="sectionHeadings">Add a New Measurement Element</h4>
            <p><asp:Literal ID="ltlNewMeasType" runat="server" /></p>
            <div style="float:left;padding-right:10px;"><telerik:RadDropDownList ID="ddlNewMeasType" runat="server" Skin="Bootstrap" Width="400px" /></div><telerik:RadButton ID="btnNewMeasType" runat="server" Text="Add new element" OnCommand="btnNewMeasType_Command" CommandArgument="addnew" AutoPostBack="true" />
            <p><asp:Label ID="lblError1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
            <asp:Label ID="lblSuccess1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="MediumOrchid"></asp:Label></p>
        </div>
    </div>

    <!-- SITE HAZARD ANALYSIS SIMS ELEMENT -->
    <div class="basicPanel">
        <h3 class="sectionHeadings">Site Hazard Analysis SIMS Element</h3>
        <p>This is information stored in the SIMS SITE HAZARD ANALYSIS element.  Please review/edit information in this element to reduce redundancy between information stored above and between
        standardized safety forms. Eventually this element will be retired.</p>
        <asp:Panel ID="pnlNoSiteHazardElem" runat="server" CssClass="roundedPanel">
            <p style="font-weight:bold;">No SITE HAZARD ANALYSIS element found in SIMS for this site.</p>
        </asp:Panel>
        <asp:Panel ID="pnlSiteHazardElem" runat="server" CssClass="roundedPanel">
            <h4 class="sectionHeadings">SITE HAZARD ANALYSIS</h4>
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
                            <telerik:EditorTool Name="AjaxSpellCheck" Visible="true" Enabled="true" />
                            <telerik:EditorTool Name="Bold" Visible="true" /> 
                            <telerik:EditorTool Name="Indent" Text="Indent" Visible="true" />
                            <telerik:EditorTool Name="Outdent" Text="Outdent" Visible="true" />
                            <telerik:EditorTool Name="InsertImage" Text="Insert Image" Visible="true" />
                            <telerik:EditorTool Name="InsertTable" Text="Insert Table" Visible="true" />
                        </telerik:EditorToolGroup>
                    </Tools>
                    <SpellCheckSettings DictionaryLanguage="en-GB" DictionaryPath="~/App_Data/RadSpell/" />
                </telerik:RadEditor>
                <br />
                <asp:Button id="btnSubmitElemInfo" runat="server" Text="save changes and leave edit mode" OnCommand="btnSubmitElemInfo_Command" CommandArgument="editelement" />
                <asp:Button ID="btnCloseElemInfoEditing" runat="server" Text="cancel and leave edit mode without saving changes" OnCommand="btnSubmitElemInfo_Command" CommandArgument="closeediting" />
                <p><asp:Label ID="lblError2" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblSuccess2" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="MediumOrchid"></asp:Label></p>
            </asp:Panel>
        </asp:Panel>
    </div>

    <!--EMERGENCY INFORMATION-->
    <div class="basicPanel">
        <h3 class="sectionHeadings">Emergency Information</h3>
        <table>
            <tr>
                <td width="400px" valign="middle"><asp:Label ID="lbl911Service" runat="server" Font-Bold="true"  /></td>
                <td><asp:LinkButton ID="lb911Service" runat="server" Text="&laquo; switch" OnCommand="lbEmergService_Command" CommandArgument="911" CommandName="servicetype" /></td>
            </tr>
            <tr>
                <td valign="middle"><asp:Label ID="lblCellService" runat="server" Font-Bold="true" /></td>
                <td valign="top"><asp:LinkButton ID="lbCellService" runat="server" Text="&laquo; switch" OnCommand="lbEmergService_Command" CommandArgument="cell" CommandName="servicetype" /></td>
            </tr>
        </table>
        <asp:Panel ID="pnlEmergencyContacts" runat="server" CssClass="roundedPanel">
            <h4 class="sectionHeadings">Manage Emergency Contacts</h4><br />
            <asp:Literal ID="ltlNotice1" runat="server" />
            <telerik:RadGrid ID="rgContacts" runat="server" AllowPaging="true"
                OnNeedDataSource="rgContacts_NeedDataSource" 
                onPreRender="rgContacts_PreRender"
                OnItemCreated="rgContacts_ItemCreated"                    
                onItemDataBound="rgContacts_ItemDataBound"
                OnInsertCommand="rgContacts_InsertCommand"
                Width="98%" Skin="Bootstrap" AllowFilteringByColumn="false">
                <SortingSettings SortToolTip="" />
                <MasterTableView AutoGenerateColumns="False" DataKeyNames="contact_id" ClientDataKeyNames="contact_id"
                    Width="100%" CommandItemDisplay="Top" AllowSorting="true" PageSize="20">
                    <CommandItemSettings AddNewRecordText="Add New Emergency Contact" ShowRefreshButton="false" />
                    <Columns>
                        <telerik:GridBoundColumn DataField="contact_id" HeaderText="Contact ID" ReadOnly="True" SortExpression="contact_id" UniqueName="contact_id" Visible="false"/>
                        <telerik:GridBoundColumn DataField="contact_nm" HeaderText="Contact Name" SortExpression="contact_nm" UniqueName="contact_nm" />
                        <telerik:GridBoundColumn DataField="street_addrs" HeaderText="Street Address" SortExpression="street_addrs" UniqueName="street_addrs" />
                        <telerik:GridBoundColumn DataField="city" HeaderText="City" SortExpression="city" UniqueName="city" HeaderStyle-Width="60" />
                        <telerik:GridBoundColumn DataField="state" HeaderText="St" SortExpression="state" UniqueName="state" HeaderStyle-Width="15" />
                        <telerik:GridBoundColumn DataField="zip" HeaderText="Zip Code" SortExpression="zip" UniqueName="zip" HeaderStyle-Width="50" />
                        <telerik:GridBoundColumn DataField="ph_no" HeaderText="Phone No." SortExpression="ph_no" UniqueName="ph_no" ItemStyle-Wrap="false" />
                        <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlDelete" runat="server" Text="Delete"></asp:HyperLink>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <div style="padding:5px;background-color:White;">
                                <table id="tableForm" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: white;">
                                    <tr>
                                        <td colspan="2">
                                            <b>Search For An Emergency Contact To Add</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <telerik:RadDropDownList ID="ddlEmergContacts" runat="server" DataTextField="contact_nm" DataValueField="contact_id" Skin="Bootstrap" Width="400px"  />
                                            <asp:RequiredFieldValidator ID="rfvEmergContacts" runat="server" ControlToValidate="ddlEmergContacts" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            <p style="font-size:8pt;color:#707070;font-style:italic;margin-top:5px;">If the contact cannot be found in the list, add it using the 
                                                <asp:HyperLink ID="hlEmergInfo2" runat="server" Target="_blank">Emergency Information Management Interface.</asp:HyperLink></p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" colspan="2">
                                            <asp:Button ID="btnInsert2" Text="Add Emergency Contact" runat="server" CommandName="PerformInsert" />
                                            <asp:Button ID="btnCancel2" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </FormTemplate>
                    </EditFormSettings>
                </MasterTableView>
            </telerik:RadGrid>
            <p style="font-size:8pt;font-style:italic;margin-top:5px;">To edit the displayed emergency contacts' information, visit the <asp:HyperLink ID="hlEmergInfo1" runat="server" Target="_blank">Emergency Information Management Interface.</asp:HyperLink></p>
        </asp:Panel>
        <asp:Panel ID="pnlHospitals" runat="server" CssClass="roundedPanel">
            <h4 class="sectionHeadings">Manage Hospitals</h4><br />
            <asp:Literal ID="ltlNotice2" runat="server" />
            <telerik:RadGrid ID="rgHospitals" runat="server" 
                AllowPaging="true" AllowSorting="true" AllowFilteringByColumn="false"
                OnNeedDataSource="rgHospitals_NeedDataSource" 
                onPreRender="rgHospitals_PreRender"
                OnItemCreated="rgHospitals_ItemCreated"
                onItemDataBound="rgHospitals_ItemDataBound"
                OnInsertCommand="rgHospitals_InsertCommand"
                Width="98%" Skin="Bootstrap">
                <SortingSettings SortToolTip="" />
                <MasterTableView AutoGenerateColumns="False" DataKeyNames="hospital_id" ClientDataKeyNames="hospital_id"
                    Width="100%" CommandItemDisplay="Top" AllowSorting="true" PageSize="20">
                    <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
                    <CommandItemSettings AddNewRecordText="Add New Hospital" ShowRefreshButton="false" />
                    <Columns>
                        <telerik:GridBoundColumn DataField="hospital_id" HeaderText="Hospital ID" ReadOnly="True" SortExpression="hospital_id" UniqueName="hospital_id" Visible="false"/>
                        <telerik:GridBoundColumn DataField="hospital_nm" HeaderText="Hospital Name" SortExpression="hospital_nm" UniqueName="hospital_nm" HeaderStyle-Width="180" />
                        <telerik:GridBoundColumn DataField="street_addrs" HeaderText="Street Address" SortExpression="street_addrs" UniqueName="street_addrs" HeaderStyle-Width="180" />
                        <telerik:GridBoundColumn DataField="city" HeaderText="City" SortExpression="city" UniqueName="city" HeaderStyle-Width="60"  />
                        <telerik:GridBoundColumn DataField="state" HeaderText="St" SortExpression="state" UniqueName="state" HeaderStyle-Width="15" />
                        <telerik:GridBoundColumn DataField="zip" HeaderText="Zip Code" SortExpression="zip" UniqueName="zip" HeaderStyle-Width="50" />
                        <telerik:GridBoundColumn DataField="ph_no" HeaderText="Phone No." SortExpression="ph_no" UniqueName="ph_no" ItemStyle-Wrap="false" />
                        <telerik:GridBoundColumn DataField="dec_lat_va" HeaderText="Decimal Lat" SortExpression="dec_lat_va" UniqueName="dec_lat_va" ItemStyle-Wrap="false" />
                        <telerik:GridBoundColumn DataField="dec_long_va" HeaderText="Decimal Long" SortExpression="dec_long_va" UniqueName="dec_long_va" ItemStyle-Wrap="false" />
                        <telerik:GridTemplateColumn UniqueName="TemplateDeleteColumn" HeaderStyle-Width="30">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlDelete" runat="server" Text="Delete"></asp:HyperLink>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>

                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <div style="padding:5px;background-color:White;">
                                <table id="tableForm" cellspacing="5" cellpadding="5" width="100%" border="0" rules="none" style="border-collapse: collapse; background: white;">
                                    <tr>
                                        <td>
                                            <b>Search For A Hospital To Add</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <telerik:RadDropDownList ID="ddlHospitals" runat="server" DataTextField="hospital_nm" DataValueField="hospital_id" Skin="Bootstrap" Width="400px" />
                                            <asp:RequiredFieldValidator ID="rfvHospitals" runat="server" ControlToValidate="ddlHospitals" ErrorMessage="*required" Font-Bold="true" Font-Size="X-Small" />
                                            <p style="font-size:8pt;color:#707070;font-style:italic;margin-top:5px;">If the hospital cannot be found in the list, add it using the <asp:HyperLink ID="hlEmergInfo3" runat="server" Target="_blank">Emergency Information Management Interface.</asp:HyperLink></p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnInsert1" Text="Add Hospital" runat="server" CommandName="PerformInsert" />
                                            <asp:Button ID="btnCancel1" Text="Cancel" runat="server" CausesValidation="false" CommandName="Cancel" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </FormTemplate>
                    </EditFormSettings>
                </MasterTableView>
            </telerik:RadGrid>
            <p style="font-size:8pt;font-style:italic;margin-top:5px;">To edit the displayed hospitals' information, visit the <asp:HyperLink ID="hlEmergInfo4" runat="server" Target="_blank">Emergency Information Management Interface.</asp:HyperLink></p>
        </asp:Panel>
        <telerik:RadWindowManager ID="rwm" runat="server">
            <Windows>
                <telerik:RadWindow ID="DeleteDialog" runat="server" Title="Delete emergency info" Height="300px" Skin="Bootstrap"
                    width="400px" Left="150px" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="true" />
            </Windows>
        </telerik:RadWindowManager>
    </div>
    
    <!-- ADMINISTRATION -->
    <div class="basicPanel">
        <h3 class="sectionHeadings">Administration</h3>
        <asp:Panel ID="pnlAdmin" runat="server" CssClass="roundedPanel">
            <h4 class="sectionHeadings">Review Info</h4>
            <p><label>Last Reviewed By:</label> <asp:Literal ID="ltlReviewedBy" runat="server" /> &nbsp;&nbsp;&nbsp;<label>Last Reviewed Date:</label> <asp:Literal ID="ltlReviewedDate" runat="server" /></p>
            <p><label>Reviewer Comments:</label><br /><asp:Literal ID="ltlReviewerComments" runat="server" /></p>
            <asp:Panel ID="pnlReview" runat="server" Visible="false">
                <p>Have you reviewed all sections of this document and want to<br />update the last reviewed by information? 
                <asp:LinkButton ID="lbReview" runat="server" Text="If yes, click here." OnCommand="lbReview_Command" CommandArgument="reviewed" /></p>
            </asp:Panel>
            <asp:Panel ID="pnlReviewSubmit" runat="server" Visible="false">
                <p style="margin-top:0;">Enter reviewer comments below, then click the button to submit as reviewed.</p>
                <asp:TextBox ID="tbReviewerComments" runat="server" Rows="5" Width="500" Height="40" TextMode="MultiLine" /><br />
                <asp:RequiredFieldValidator ID="rfvReviewerComments" runat="server" ControlToValidate="tbReviewerComments" ErrorMessage="You must enter a comment." Font-Bold="true" ForeColor="Red" /><br />
                <asp:Button ID="btnReviewed" runat="server" Text="Submit Document as Reviewed" OnCommand="btnAdmin_Command" CommandArgument="Reviewed" />
                <asp:Button ID="btnCancelReview" runat="server" Text="Cancel" OnCommand="btnAdmin_Command" CommandArgument="CancelReview" CausesValidation="false" />
            </asp:Panel>
            <br />
            <h4 class="sectionHeadings">Approve Info</h4>
            <p><label>Last Approved By:</label> <asp:Literal ID="ltlApprovedBy" runat="server" /> &nbsp;&nbsp;&nbsp;<label>Last Approved Date:</label> <asp:Literal ID="ltlApprovedDate" runat="server" /></p>
            <asp:Panel ID="pnlApprove" runat="server" Visible="false">
                <asp:Panel ID="pnlApprovePreSubmit" runat="server">
                    <p>Have you approved all sections of this document and want to<br />mark this document as approved?
                    <asp:LinkButton ID="lbApprove" runat="server" Text="If yes, click here." OnCommand="lbApprove_Command" CommandArgument="approved" /></p>
                </asp:Panel>
                <asp:Panel ID="pnlApproveSubmit" runat="server" Visible="false">
                    <p style="margin-top:0;">
                    <asp:Button ID="btnApproved" runat="server" Text="Submit Document as Approved" OnCommand="btnAdmin_Command" CommandArgument="Approved" />
                    <asp:Button ID="btnCancelApprove" runat="server" Text="Cancel" OnCommand="btnAdmin_Command" CommandArgument="CancelApprove" />
                    </p>
                </asp:Panel>
            </asp:Panel>
            <br />
        </asp:Panel>
    </div>
    
</asp:Panel>
</asp:Content>
