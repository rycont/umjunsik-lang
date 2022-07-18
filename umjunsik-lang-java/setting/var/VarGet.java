package setting.var;

import setting.Count;
import setting.Items;

public class VarGet {
    //어..
    public int get(String line) {
        Count count = new Count();
        return count.count(changeVar(line.trim()));
    }

    public boolean check(String text) {
        return text.contains("어");
    }

    //변수 -> 값
    private String changeVar(String line) {
        int len = line.trim().replaceAll("[.|,]", "").length();
        for (int i = len; i>0; i--) {
            String key = getKey(i);
            if (line.contains(key)) line = line.replace(key, Items.map.get(i));
        } return line;
    }

    private String getKey(int len) {
        String total = "";
        for (int i = 0; i<len; i++) total += "어";
        return total;
    }
}
