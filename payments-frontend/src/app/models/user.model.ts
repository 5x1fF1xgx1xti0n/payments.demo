import { Account } from './account.model';

export class User {
    public id!: number;
    public fullName!: string;
    public login!: string;
    public role!: string;
    public account!: Account;
}