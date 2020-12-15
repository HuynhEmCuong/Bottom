import { INavData } from '@coreui/angular';
import { Injectable } from '@angular/core';

export const navItems: INavData[] = [];

@Injectable({
  providedIn: 'root'  // <- ADD THIS
})
export class NavItem {
  navItems: INavData[] = [];
  hasMaintain: boolean;
  hasTransaction: boolean;
  hasKanban: boolean;
  hasReport: boolean;
  hasQuery: boolean;

  constructor() { }

  getNav() {
    this.navItems = [];
    const user: any = JSON.parse(localStorage.getItem('user'));

    //#region "navMaintain"
    const navMaintain = {
      name: '1. Maintain',
      url: '/rack',
      icon: 'fa fa-cogs',
      children: []
    };
    const navMaintain_RackLocation = {
      name: '1.1 Rack Location',
      url: '/rack/main',
      class: 'menu-margin',
    };
    const navMaintain_T3Supplier = {
      name: '1.2 T3 Supplier',
      url: '/rack/setting-mail/list-mail',
      class: 'menu-margin',
    };
    if (user.role.includes('wmsb.RackLocationMain') === true) {
      navMaintain.children.push(navMaintain_RackLocation);
      this.hasMaintain = true;
    }
    if (user.role.includes('wmsb.SettingT3Supplier') === true) {
      navMaintain.children.push(navMaintain_T3Supplier);
      this.hasMaintain = true;
    }
    //#endregion

    //#region "navTransaction"
    const navTransaction = {
      name: '2. Transaction',
      url: '/transaction',
      icon: 'fa fa-balance-scale',
      children: []
    };
    const navTransaction_Receiving = {
      name: '2.1 Receiving Material',
      url: 'receiving',
      class: 'menu-margin',
      children: []
    };
    let hasReceiving;
    const navTransaction_Receiving_A = {
      name: '2.1.1 Receiving Mat# A',
      url: '/receipt/main',
      class: 'menu-child-margin',
    };
    const navTransaction_Receiving_B = {
      name: '2.1.2 Receiving Mat# B',
      url: '/receive/main',
      class: 'menu-child-margin',
    };
    const navTransaction_QrGeneratePrint = {
      name: '2.2 QR Code Generate',
      url: '/qr/main',
      class: 'menu-margin',
    };
    const navTransaction_IntegrationInput = {
      name: '2.3 QR Generate (Integration)',
      url: '/qr/integration',
      class: 'menu-margin',
    };
    const navTransaction_InputMain = {
      name: '2.4 Input',
      url: '/input/main',
      class: 'menu-margin',
    };
    const navTransaction_GenerateCollectionTransferForm = {
      name: '2.5 Transfer Form Generate',
      url: '/input/transfer-form/genrate',
      class: 'menu-margin',
    };
    const navTransaction_Output = {
      name: '2.6 Out',
      url: '/output/main',
      class: 'menu-margin',
    };
    const navTransaction_TransferLocationMain = {
      name: '2.7 Transfer Location',
      url: '/transfer/main',
      class: 'menu-margin',
    };

    if (user.role.includes('wmsb.ReceivingMaterial.A') === true) {
      navTransaction_Receiving.children.push(navTransaction_Receiving_A);
      hasReceiving = true;
    }
    if (user.role.includes('wmsb.ReceivingMaterial.B') === true) {
      navTransaction_Receiving.children.push(navTransaction_Receiving_B);
      hasReceiving = true;
    }
    if (hasReceiving) {
      navTransaction.children.push(navTransaction_Receiving);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.QrGeneratePrint') === true) {
      navTransaction.children.push(navTransaction_QrGeneratePrint);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.IntegrationInput') === true) {
      navTransaction.children.push(navTransaction_IntegrationInput);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.InputMain') === true) {
      navTransaction.children.push(navTransaction_InputMain);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.GenerateCollectionTransferForm') === true) {
      navTransaction.children.push(navTransaction_GenerateCollectionTransferForm);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.Output') === true) {
      navTransaction.children.push(navTransaction_Output);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.TransferLocationMain') === true) {
      navTransaction.children.push(navTransaction_TransferLocationMain);
      this.hasTransaction = true;
    }
    //#endregion

    //#region "navKanban"
    const navKanban = {
      name: '3. Kanban',
      url: '/kanban',
      icon: 'fa fa-desktop',
      children: []
    };
    const navKanban_Kanban = {
      name: '3.1 Kanban',
      url: '/kanban/',
      class: 'menu-margin',
      attributes: { target: '_blank' },
    };
    if (user.role.includes('wmsb.Kanban') === true) {
      navKanban.children.push(navKanban_Kanban);
      this.hasKanban = true;
    }
    //#endregion

    //#region "navReport"
    const navReport = {
      name: '4. Report',
      url: '/report',
      icon: 'fa fa-newspaper-o',
      children: []
    };
    const navReport_Report = {
      name: '4.1 Receive Material Report',
      url: '/report/receive-material',
      class: 'menu-margin',
    };
    if (user.role.includes('wmsb.ReceiveMaterialReport') === true) {
      navReport.children.push(navReport_Report);
      this.hasReport = true;
    }
    //#endregion

    //#region "navQuery"
    const navQuery = {
      name: '5. Query',
      url: '/query',
      icon: 'fa fa-search',
      children: []
    };
    const navQuery_History = {
      name: '5.1 History',
      url: '/transfer/history',
      class: 'menu-margin',
    };
    const navQuery_QrGeneratePrint = {
      name: '5.2 Material Form Print',
      url: '/qr/body',
      class: 'menu-margin',
    };
    const navQuery_QrPrintAgain = {
      name: '5.3 Sorting Form Print',
      url: '/input/qrcode-again',
      class: 'menu-margin',
    };
    const navQuery_MissingPrint = {
      name: '5.4 Missing Print',
      url: '/input/missing-again',
      class: 'menu-margin',
    };
    const navQuery_PrintTransferForm = {
      name: '5.5 Transfer Form (Email/Print/Release)',
      url: '/input/transfer-form/print',
      class: 'menu-margin',
    };
    const navQuery_CompareReport = {
      name: "5.6 Compare Report",
      url: "/report/compare-report",
      class: "menu-margin",
    };
    if (user.role.includes('wmsb.InOutHistory') === true) {
      navQuery.children.push(navQuery_History);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.QrGeneratePrint') === true) {
      navQuery.children.push(navQuery_QrGeneratePrint);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.QrPrintAgain') === true) {
      navQuery.children.push(navQuery_QrPrintAgain);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.MissingPrint') === true) {
      navQuery.children.push(navQuery_MissingPrint);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.PrintTransferForm') === true) {
      navQuery.children.push(navQuery_PrintTransferForm);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.CompareReport') === true) {
      navQuery.children.push(navQuery_CompareReport);
      this.hasQuery = true;
    }
    //#endregion

    if (this.hasMaintain) {
      this.navItems.push(navMaintain);
    }
    if (this.hasTransaction) {
      this.navItems.push(navTransaction);
    }
    if (this.hasKanban) {
      this.navItems.push(navKanban);
    }
    if (this.hasReport) {
      this.navItems.push(navReport);
    }
    if (this.hasQuery) {
      this.navItems.push(navQuery);
    }
    return this.navItems;
  }
}
