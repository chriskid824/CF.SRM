import { Component } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular/ag-grid-angular';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'date-editor-cell',
  template: `
    <nz-date-picker nzSize="large" nzAutoFocus="true" (select)="onDateSelect($event)" style nzPlaceHolder="請選擇日期"></nz-date-picker>
  `
})
export class AgGridDatePickerComponent implements ICellEditorAngularComp {
  private params: any;

  public selectedDate: any;

  agInit(params: any): void {
    this.params = params;

  }

  getValue(): any {
    return this.selectedDate;
  }

  isPopup(): boolean {
    return true;
  }

  onDateSelect(date: Date) {
    this.selectedDate = date.getDate();//{ date: { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDay() } };
    this.params.api.stopEditing();
  }
}
