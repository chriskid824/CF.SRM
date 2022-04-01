import { Component } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular/ag-grid-angular';
import { DatePipe } from '@angular/common';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { differenceInCalendarDays } from 'date-fns';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { PoReasonModalComponent} from './po-date-modal';
@Component({
  selector: 'date-editor-cell',
  template: `
    <nz-date-picker nzSize="large" *ngIf="(params.data.Status==15||params.data.Status==11)&&params.data.Org!=3100" nzAutoFocus="true" [(ngModel)]="selectedDate" (ngModelChange)="onChange($event)" style nzPlaceHolder="請選擇日期" [nzDisabledDate]="disabledDate"></nz-date-picker>
    `
})
export class AgGridDatePickerComponent implements ICellEditorAngularComp {
  constructor(private _srmPoService: SrmPoService,private dialog: MatDialog) {
  }
  public params: any;

  public selectedDate: any;
  public isshow:boolean=false;
  disabledDate= (current: Date): boolean =>
  // Can not select days before today and today
  differenceInCalendarDays(current, new Date(this.params.data.DeliveryDate)) < -7;
  agInit(params: any): void {
    this.params = params;
  }
  ngAfterViewInit() {
    if(this.params.data.Org==3100)
    {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = true;
      dialogConfig.autoFocus = true;
      //dialogConfig.minWidth = "1500px";
      dialogConfig.maxHeight = "1500px";
      dialogConfig.data = this.params;
      this.dialog.open(PoReasonModalComponent, dialogConfig);
    }
  }
  getValue(): any {
    if(this.selectedDate!=undefined)
    {
      this.params.data.ReplyDeliveryDate=this.selectedDate;
      this._srmPoService.UpdateReplyDeliveryDate(this.params.data).subscribe(result => {
        this.params.data.Status=15;
        this.params.data.StatusDesc="待交貨";
        this.params.api.refreshCells();
      });
    }
    return this.params.data.ReplyDeliveryDate;
  }

  isPopup(): boolean {
    return true;
  }

  onChange(date: Date) {
    this.selectedDate = date;//{ date: { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDay() } };
    this.params.api.stopEditing();
    console.info(this.params);
  }
}





