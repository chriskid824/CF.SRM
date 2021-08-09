import { Component } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular/ag-grid-angular';
import { DatePipe } from '@angular/common';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { differenceInCalendarDays } from 'date-fns';

@Component({
  selector: 'date-editor-cell',
  template: `
    <nz-date-picker nzSize="large" nzAutoFocus="true" [(ngModel)]="selectedDate" (ngModelChange)="onChange($event)" style nzPlaceHolder="請選擇日期" [nzDisabledDate]="disabledDate"></nz-date-picker>
  `
})
export class AgGridDatePickerComponent implements ICellEditorAngularComp {
  constructor(private _srmPoService: SrmPoService) {
  }
  private params: any;

  public selectedDate: any;
  disabledDate= (current: Date): boolean =>
  // Can not select days before today and today
  differenceInCalendarDays(current, new Date(this.params.data.DeliveryDate)) < -7;
  agInit(params: any): void {
    this.params = params;
  }

  getValue(): any {
    this.params.data.ReplyDeliveryDate=this.selectedDate;
    this._srmPoService.UpdateReplyDeliveryDate(this.params.data).subscribe(result => {
      this.params.data.status=12;
      this.params.api.refreshCells();
    });
    return this.selectedDate;
  }

  isPopup(): boolean {
    return true;
  }

  onChange(date: Date) {
    this.selectedDate = date;//{ date: { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDay() } };
    this.params.api.stopEditing();
  }
}
