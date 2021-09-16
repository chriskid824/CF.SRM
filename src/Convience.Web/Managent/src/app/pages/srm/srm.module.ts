import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AngularDualListBoxModule } from 'angular-dual-listbox';
import { RouterModule } from '@angular/router';
import { RfqComponent } from './rfq/rfq.component';
import { LoginGuard } from 'src/app/guards/login.guard';
import { AgGridModule } from 'ag-grid-angular';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { RfqManageComponent } from './rfq-manage/rfq-manage.component';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { AppCommonModule } from '../app-common/app-common.module';
import { MatTabsModule } from '@angular/material/tabs';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { PoComponent } from './po/po.component';
import { QotComponent } from './qot/qot.component';
import { AgGridDatePickerComponent } from './po/AGGridDatePickerCompponent';
import 'ag-grid-enterprise';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { PriceWorkComponent } from './price-work/price-work.component';
import { PriceComponent } from './price/price.component';
import { PriceManageComponent } from './price-manage/price-manage.component';
import { PoDetailComponent } from './po-detail/po-detail.component';
import { QotlistComponent } from './qotlist/qotlist.component';
import { DelyveryLComponent } from './delyvery-l/delyvery-l.component';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import { NzIconModule } from 'ng-zorro-antd/icon';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { NzTreeModule } from 'ng-zorro-antd/tree';
import { DeliveryModalComponent } from './delivery-modal/delivery-modal.component';
import { MatDialogModule } from "@angular/material/dialog";
import { QRCodeModule } from 'angularx-qrcode';
import { DeliveryReceiveComponent } from './delivery-receive/delivery-receive.component';
import { ButtonRendererComponent } from './price/button-cell-renderer.component';
import { EditButtonComponent } from './delyvery-l/button-cell-renderer.component';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { DeliveryAddComponent } from './delivery-add/delivery-add.component';
import {MatButtonModule} from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { SupplierComponent } from './supplier/supplier.component';
import { SupplierCComponent } from './supplier-c/supplier-c.component';
import { MaterialComponent } from './material/material.component';
import { MaterialCComponent } from './material-c/material-c.component';
@NgModule({
  declarations: [
    RfqComponent,
    RfqManageComponent,
    PoComponent,
    AgGridDatePickerComponent,
    QotComponent,
    PriceWorkComponent,
    PriceComponent,
    PriceManageComponent,
    PoDetailComponent,
    QotlistComponent,
    DelyveryLComponent,
    DeliveryModalComponent,
    DeliveryReceiveComponent,
    ButtonRendererComponent,
    EditButtonComponent,
    DeliveryAddComponent,
    SupplierComponent,
    SupplierCComponent,
    MaterialComponent,
    MaterialCComponent,
  ],
  imports: [
    CommonModule,
    AngularDualListBoxModule,
    RouterModule.forChild([
       { path: '', pathMatch: 'full', redirectTo: 'rfq' },
       { path: "rfq", component: RfqComponent, canActivate: [LoginGuard] },
       { path: "rfq-manage", component: RfqManageComponent, canActivate: [LoginGuard] },
       { path: "po", component: PoComponent , canActivate: [LoginGuard]},
       { path: "deliveryh", component: PoDetailComponent },
       { path: "deliveryl", component: DelyveryLComponent },
       { path: "deliverymodal", component: DeliveryModalComponent },
       { path: "deliveryreceive", component: DeliveryReceiveComponent },
       { path: "qot", component: QotComponent },
       { path: "qotlist", component: QotlistComponent, canActivate: [LoginGuard] },
       //{ path: "qotlist", component: QotlistComponent},
       { path: "price-work", component: PriceWorkComponent, canActivate: [LoginGuard] },
       { path: "price", component: PriceComponent, canActivate: [LoginGuard] },
       { path: "price-manage", component: PriceManageComponent, canActivate: [LoginGuard] },
       { path: "supplier", component: SupplierComponent, canActivate: [LoginGuard] },
       { path: "supplier-c", component: SupplierCComponent, canActivate: [LoginGuard] },
       { path: "material", component: MaterialComponent, canActivate: [LoginGuard] },
       { path: "material-c", component: MaterialCComponent, canActivate: [LoginGuard] }
    ]),
    AgGridModule.withComponents([ButtonRendererComponent]),
    NzButtonModule,
    NzFormModule,
    NzCardModule,
    NzTableModule,
    NzPaginationModule,
    NzCheckboxModule,
    NzRadioModule,
    AppCommonModule,
    MatTabsModule,
    NzDatePickerModule,
    NzSelectModule,
    MatFormFieldModule,
    MatSelectModule,
    NzIconModule,
    MatCheckboxModule,
    NzTreeModule,
    MatDialogModule,
    QRCodeModule,
    ZXingScannerModule,
    NzModalModule,
    NzInputModule,
    NzToolTipModule,
    MatButtonModule,
    MatInputModule,
  ]
})
export class SrmModule {
  static decimal = /^(0|([1-9](\d)*))(\.(\d)*)?$/;
  static decimalTwoDigits = /^(0|([1-9](\d)*))(\.(\d){1,2})?$/;
  static number = /^(0|([1-9](\d)*))$/;
}
