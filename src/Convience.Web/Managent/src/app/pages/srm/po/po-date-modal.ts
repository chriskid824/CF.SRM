import { Component,OnInit,Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { ICellEditorAngularComp } from 'ag-grid-angular/ag-grid-angular';
import { DatePipe } from '@angular/common';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { differenceInCalendarDays } from 'date-fns';

@Component({
  selector: 'po-date-modal',
  template: `
  <h3 mat-dialog-title>{{data}}</h3>
  <mat-dialog-content>
    <nz-date-picker nzSize="large" nzAutoFocus="true" [(ngModel)]="selectedDate" (ngModelChange)="onChange($event)" style nzPlaceHolder="請選擇日期"></nz-date-picker>
  </mat-dialog-content>
  <mat-dialog-actions align="end">
    <button mat-button mat-dialog-close mat-raised-button nbButton style="margin-right: 20px;">關閉</button>
    <button mat-button cdkFocusInitial (click)="save()">保存</button>
  </mat-dialog-actions>
  `
})
export class PoDateModalComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,private _srmPoService: SrmPoService) {
  }
  public params: any;

  public selectedDate: any;
  public isshow:boolean=false;
  disabledDate= (current: Date): boolean =>
  // Can not select days before today and today
  differenceInCalendarDays(current, new Date(this.params.data.DeliveryDate)) < -7;
  agInit(params: any): void {
    this.params = params;
    console.info(params);
  }
  ngOnInit(): void {
  }
  save(): any {
    if(this.selectedDate!=undefined)
    {
      var query = {
        PoNum: this.data,
        date: this.selectedDate,
      }
      // this.params.data.ReplyDeliveryDate=this.selectedDate;
      this._srmPoService.UpdateReplyDeliveryDateH(query).subscribe(result => {
        // this.params.data.Status=15;
        // this.params.data.StatusDesc="待交貨";
        // this.params.api.refreshCells();
        location.reload();
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
  }
}

@Component({
  selector: 'po-reason-modal',
  template: `
  <h3 mat-dialog-title>{{data.data.PoNum}}-{{data.data.PoLId}}</h3>
  <mat-dialog-content>
    <nz-date-picker nzSize="large" nzAutoFocus="true" [(ngModel)]="selectedDate" (ngModelChange)="onChange($event)" style nzPlaceHolder="請選擇日期"></nz-date-picker>
    <textarea (ngModelChange)="onReasonChange($event)"  style="margin-top:20px" [(ngModel)]="reason" nz-input placeholder="交貨異動說明" [nzAutosize]="{ minRows: 3, maxRows: 5 }"></textarea>
  </mat-dialog-content>
  <mat-dialog-actions align="end">
    <button mat-button mat-dialog-close mat-raised-button nbButton style="margin-right: 20px;">關閉</button>
    <button mat-button cdkFocusInitial (click)="save()" [disabled]="reason==undefined||reason==''">保存</button>
  </mat-dialog-actions>
  `
})

export class PoReasonModalComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,private _srmPoService: SrmPoService,public dialogRef: MatDialogRef<PoReasonModalComponent>) {
  }
  public params: any;

  public selectedDate: any;
  public reason: any;
  public isshow:boolean=false;
  disabledDate= (current: Date): boolean =>
  // Can not select days before today and today
  differenceInCalendarDays(current, new Date(this.data.data.DeliveryDate)) < -7;
  agInit(params: any): void {
    this.params = params;
  }
  ngOnInit(): void {
  }
  save(): any {
    if(this.selectedDate!=undefined)
    {
      var query = {
        PoId : this.data.data.PoId,
        PoLId : this.data.data.PoLId,
        PoNum : this.data.data.PoNum,
        date : this.selectedDate,
        Reason : this.reason,
      }
      this._srmPoService.UpdateReplyDeliveryDateWithReason(query).subscribe(result => {
        alert("修改成功");
        this.data.data.LastReplyDeliveryDate=this.data.data.ReplyDeliveryDate;
        this.data.data.ReplyDeliveryDate=this.selectedDate;
        this.data.data.ChangeDateReason=this.reason;
        this.data.api.refreshCells();
        this.dialogRef.close();
      });
    }
    return this.data.data.ReplyDeliveryDate;
  }

  isPopup(): boolean {
    return true;
  }

  onChange(date: Date) {
    this.selectedDate = date;//{ date: { year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDay() } };
    //this.params.api.stopEditing();
    console.info(this.reason);
  }
  onReasonChange(reason) {
  }
}

