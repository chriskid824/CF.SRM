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
import { AppCommonModule } from '../app-common/app-common.module';
import { MatTabsModule } from '@angular/material/tabs';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';

@NgModule({
  declarations: [
    RfqComponent,
    RfqManageComponent,
  ],
  imports: [
    CommonModule,
    AngularDualListBoxModule,
    RouterModule.forChild([
      { path: '', pathMatch: 'full', redirectTo: 'rfq' },
      { path: "rfq", component: RfqComponent, canActivate: [LoginGuard] },
      { path: "rfq-manage", component: RfqManageComponent, canActivate: [LoginGuard] }
    ]),
    AgGridModule.withComponents([]),
    NzButtonModule,
    NzFormModule,
    NzCardModule,
    NzTableModule,
    NzPaginationModule,
    NzCheckboxModule,
    AppCommonModule,
    MatTabsModule,
    NzDatePickerModule
  ]
})
export class SrmModule { }
