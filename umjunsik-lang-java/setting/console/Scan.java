package setting.console;

import setting.var.VarSet;

import java.util.Scanner;

public class Scan {
    public void scanner(String line) {
        Scanner sc = new Scanner(System.in);
        int getInt = sc.nextInt();
        VarSet varSet = new VarSet();
        varSet.set(line, getInt);
    }

    public boolean check(String line) {
        return line.trim().endsWith("식?");
    }
}
