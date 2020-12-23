export class Transaction {
    public id!: number;
    public date!: Date;
    public rawSum!: number;
    public commissionSum!: number;
    public totalSum!: number;
    public fromAccountNumber!: string;
    public toAccountNumber!: string;
}