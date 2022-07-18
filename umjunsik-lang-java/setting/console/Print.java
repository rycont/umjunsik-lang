package setting.console;

import setting.var.VarGet;

public class Print {

    public void print(String line) {

        int start = line.indexOf("식") + 1;
        int end = line.lastIndexOf("!");

        line = line.substring(start, end);
        VarGet varGet = new VarGet();
        System.out.print(varGet.get(line));
    }

    public boolean check(String line) {
        return line.trim().startsWith("식") && line.endsWith("!");
    }
}
