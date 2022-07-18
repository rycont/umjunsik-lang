import setting.Items;
import setting.console.ChangeUni;
import setting.console.Print;
import setting.console.Scan;
import setting.var.VarGet;
import setting.var.VarSet;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;
import java.util.Locale;

public class main extends Items {

    public static void main(String[] args) throws IOException {
        TotalText = "";

//        Path path = Paths.get("./" + "test.umm");
//        Charset cs = StandardCharsets.UTF_8;
//        List<String> list = Files.readAllLines(path, cs);
//        list.forEach(main::first);

        String fileName = args[0];
        if (fileName.toLowerCase(Locale.ROOT).endsWith(".umm")) throw new IOException("확장자가 일치하지 않습니다.");
        BufferedReader reader = new BufferedReader(new FileReader(fileName, StandardCharsets.UTF_8));
        String readerString;
        while ((readerString = reader.readLine()) != null) first(readerString);
        reader.close();

        int startPos = TotalText.indexOf("어떻게") + 3;
        int endPos = TotalText.lastIndexOf("이 사람이름이냐ㅋㅋ");
        TotalText = TotalText.substring(startPos, endPos);

        ChangeUni changeUni = new ChangeUni();
        Print print = new Print();
        Scan scan = new Scan();
        VarGet varGet = new VarGet();
        VarSet varSet = new VarSet();

        String[] lines = TotalText.split("\\n");
        for (String line : lines) {
//            System.out.println(line);
            if (changeUni.check(line)) changeUni.change(line);
            else if (print.check(line)) print.print(line);
            else if (scan.check(line)) scan.scanner(line);
            else if (varGet.check(line)) varGet.get(line);
            else if (varSet.check(line)) varSet.set(line);
        }
    }

    private static void first(String line) {
        TotalText += (line + "\n");
    }
}
