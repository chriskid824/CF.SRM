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
import { PriceManageComponent } from './price-manage/price-manage.component';
import { PriceComponent } from './price/price.component';
import { PoDetailComponent } from './po-detail/po-detail.component';
import { QotlistComponent } from './qotlist/qotlist.component';
@NgModule({
  declarations: [
    RfqComponent,
    RfqManageComponent,
    PoComponent,
    AgGridDatePickerComponent,
    QotComponent,
    PriceManageComponent,
    PriceComponent,
    PoDetailComponent,
    QotlistComponent
  ],
  imports: [
    CommonModule,
    AngularDualListBoxModule,
    RouterModule.forChild([
      //{ path: '', pathMatch: 'full', redirectTo: 'rfq' },
      { path: "rfq", component: RfqComponent, canActivate: [LoginGuard] },
      { path: "rfq-manage", component: RfqManageComponent, canActivate: [LoginGuard] },
      { path: "po", component: PoComponent },
      { path: "podetail", component: PoDetailComponent },
      { path: "qot", component: QotComponent },
      { path: "qotlist", component: QotlistComponent, canActivate: [LoginGuard] },
      { path: "price-manage", component: PriceManageComponent, canActivate: [LoginGuard] },
      { path: "price", component: PriceComponent, canActivate: [LoginGuard] }
    ]),
    AgGridModule.withComponents([]),
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
    NzSelectModule
  ]
})
export class SrmModule { }
