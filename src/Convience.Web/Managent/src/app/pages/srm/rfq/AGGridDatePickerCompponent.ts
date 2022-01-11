import { Component } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular/ag-grid-angular';
import { DatePipe } from '@angular/common';
import { differenceInCalendarDays } from 'date-fns';

@Component({
  selector: 'date-editor-cell',
  template: `
    <nz-date-picker nzSize="large" nzAutoFocus="true" [(ngModel)]="selectedDate" (ngModelChange)="onChange($event)" style nzPlaceHolder="請選擇日期" [nzDisabledDate]="disabledDate"></nz-date-picker>
  `
})
export class AgGridDatePickerComponent_RFQ implements ICellEditorAngularComp {
  constructor() {
  }
  private params: any;

  public selectedDate: any;
  disabledDate= (current: Date): boolean =>
  // Can not select days before today and today
  differenceInCalendarDays(current, new Date(this.params.data.date)) < -7;
  agInit(params: any): void {
    this.params = params;
  }

  getValue(): any {
    return `${this.selectedDate.getFullYear()}-${this.selectedDate.getMonth() + 1}-${this.selectedDate.getDate()}`;
  }

  isPopup(): boolean {
    return true;
  }

  onChange(date: Date) {
    //console.log(this.params);
    this.selectedDate = date;//{ date: { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDay() } };
    this.params.api.stopEditing();
  }
}
