package setting.console;

import setting.var.VarGet;

public class ChangeUni {

    public void change(String line) {
        int start = line.indexOf("식") + 1;
        int end = line.lastIndexOf("ㅋ");
        line = line.substring(start, end);

        VarGet varGet = new VarGet();
        if (line.isBlank()) System.out.println();
        else {
            int i = varGet.get(line);
            System.out.print((char) i);
        }
    }

    public boolean check(String line) {
        if (line.isBlank()) return false;
        return line.trim().startsWith("식") && line.endsWith("ㅋ");
    }
}
