//taken from https://answers.unity.com/questions/433283/how-to-send-email-with-c.html

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

[CreateAssetMenu(fileName = "MailLog", menuName = "Mail Log")]
public class mono_gmail : ScriptableObject
{
    public string sender = "";
    public string pass = "";
    public string recipient = "";

    [TextArea]
    public string log = "";

    public void LogClick(GameObject clickable)
    {
        string log = "OnClick: " + clickable.name + " [child of ";
        string parent = clickable.transform.parent != null ? clickable.transform.parent.name : "None";
        log += parent + "]" + " at " + Time.timeSinceLevelLoad;

        this.log += log + "\n";
    }

    public void LogSurvey(Survey survey)
    {
        log += "\n" + survey.name;
        for(int i = 0; i < survey.frames.Count; i++)
        {
            log += "\n" + i + " > " + survey.frames[i].question.text + "\n>> ";

            switch(survey.questions[i].questionType)
            {
                case QuestionType.multipleChoice:
                    foreach(Toggle t in survey.frames[i].toggleGroup.GetComponentsInChildren<Toggle>())
                    {
                        if(t.isOn)
                        {
                            log += t.GetComponentInChildren<Text>().text;
                            break;
                        }
                    }
                    break;
                case QuestionType.shortAnswer:
                    log += survey.frames[i].shortResponseField.text;
                    break;
                case QuestionType.longAnswer:
                    log += survey.frames[i].longResponseField.text;
                    break;
            }

        }

        log += "\n\nTime since app start: " + Time.timeSinceLevelLoad + "\n\n";
    }

    public void LogTimeSinceAppStart()
    {
        log += "\n\nTime since app start: " + Time.timeSinceLevelLoad + "\n\n";
    }

    public void SendMail(string subject, string body)
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(sender);
        mail.To.Add(recipient);
        mail.Subject = subject;
        mail.Body = body;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(sender, pass) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");
    }

    public void SendMail()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(sender);
        mail.To.Add(recipient);
        mail.Subject = "BlueJam Log";

        mail.Body = log;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(sender, pass) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");
    }
}
